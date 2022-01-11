using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{
    public float jumpForce;
    public bool isGrounded; //public for testing purposes, change to private once done
    public LayerMask whatIsGround; //Checks to see if the Layer is marked "ground"
    public int initBottles = 24; //set to private once testing is over  //Starting Bottle count
    public float initBoozeTimer = 5f; //set to private once testing is over  //Starting BoozeTimer
    public int minBottleReq = 10; //The minimal bottle requirement for the pitstop

    private bool isWin = false;
    private Vector3 initPos; //Starting or checkpoint Player Position
    private float boozeTimer; //Current Booze Timer
    private int bottles; //Current Bottle count
    private Rigidbody2D rb; //The player's Rigidbody
    [SerializeField] private CapsuleCollider2D runCollider; //The player's running collider
    [SerializeField] private CapsuleCollider2D slideCollider; //the player's slide collider
    private Animator anim; //The Player's animator
    private bool gameStarted; //Is the game currently going?
    private bool boozedUp; //Is the player currently in Booze Power mode?
    private bool dead = false; //Well pretty self explanatory
    private bool isSliding = false;
    private bool isPunching = false;
    private bool readyThrow = true;
    private bool gameEnd = false;
    private SFXManager sfxManager;
    private int jumpSFXNum;
    private PlatformMover[] startPlatforms; //All the platforms currently in the scene
    private EnemyBehaviour[] enemies; //All the enemies currently in the scene
    private BottlePickup[] bottleCollectibles;
    private LevelManager levelMng;
    private ScoreMaster scoreMaster;

    //Bunch of variables I want to see in editor but not change



    [SerializeField]
    private GameObject bottleThrown;
    [SerializeField]
    private Transform throwingArm;
    [SerializeField]
    private GameObject colRunning; //The running collider
    [SerializeField]
    private GameObject colSliding; //The sliding collider
    [SerializeField]
    private GameObject colBoozePower; //The booze power collider
    [SerializeField]
    private GameObject colPunch;
    [SerializeField]
    private CapsuleCollider2D colTriggerRun;
    [SerializeField]
    private CapsuleCollider2D colTriggerSlide;
    [SerializeField]
    private Text bottleCountText; //The bottle count text............
    [SerializeField]
    private GameObject startText;
    [SerializeField]
    private GameObject deathText;
    [SerializeField]
    private Text textPrompt;
    [SerializeField]
    private GameObject endScreen;
    [SerializeField]
    private GameObject enemyActivation;
    private Text endScreenTxt;


    void Start()
    {

        //Yeh git those components
        rb = GetComponent<Rigidbody2D>();
        runCollider = colRunning.GetComponent<CapsuleCollider2D>();
        slideCollider = colSliding.GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        startPlatforms = GameObject.FindObjectsOfType<PlatformMover>();
        enemies = GameObject.FindObjectsOfType<EnemyBehaviour>();
        bottleCollectibles = GameObject.FindObjectsOfType<BottlePickup>();
        sfxManager = GameObject.FindObjectOfType<SFXManager>();
        endScreenTxt = endScreen.GetComponentInChildren<Text>();
        levelMng = GameObject.FindObjectOfType<LevelManager>();
        scoreMaster = GameObject.FindObjectOfType<ScoreMaster>();
        jumpSFXNum = sfxManager.jumpSFX.Length;
        startText.SetActive(true);
        deathText.SetActive(false);
        endScreen.SetActive(false);
        textPrompt.gameObject.SetActive(false);
        enemyActivation.SetActive(true);

        //Initializes the timer, count and position (Should this be in the Initialize function? TBD)
        boozeTimer = initBoozeTimer;
        bottles = initBottles;
        initPos = transform.position;

        UpdateBottleCountDisplay(); //Updates...the Bottle...Count...Display
        Initialize(); //Resets animations to Idle and pauses the game until StartGame input
    }

    void FixedUpdate()
    {
        //What is ground????? This checks to see if the player is touching it!    
        if (!isSliding)
        {
            isGrounded = Physics2D.IsTouchingLayers(runCollider, whatIsGround);
        }
        else
        {
            isGrounded = Physics2D.IsTouchingLayers(slideCollider, whatIsGround);
        }

        anim.SetBool("isGrounded", isGrounded); //Makes sure the animations go back to normal if the player touches the ground

    }

    void Update()
    {

        if (!gameStarted) //Checks to see if the game is set to "pause" mode
        {
            PauseGame();
            if (Input.GetKeyDown(KeyCode.Space)) //Change keycode to something more appropriate once game is near completion
            {
                startText.SetActive(false);
                StartRunning(); //Starts the game!
            }
        }
        else if (dead) //Checks to see if you died a horrible painful death
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ResetEverything(); //Three guesses as to what this does
            }
        }
        else if (!gameEnd)//This should be the default setting, when the game is running
        {
            Jump(); //Lets you soar through the air without a care in the world
            Slide(); //Lets you slide real smooth like
            ActivateBoozePower(); //Activates the wonderful magical power of BOOZE
            Throw();
            Punch();

            //The booze power timer
            if (boozedUp) //Are you drunk?
            {
                boozeTimer -= Time.deltaTime;

                if (boozeTimer <= 0)
                {
                    DeactivateBoozePower(); //You are now sober/sad
                }
            }

        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isWin)
                {
                    if (SceneManager.GetActiveScene().name.Contains("Level 3"))
                    {
                        levelMng.LoadLevel("Endgame");
                    }
                    else
                    {
                        levelMng.LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
                    }
                }
                else
                {
                    levelMng.LoadLevel(SceneManager.GetActiveScene().buildIndex);
                }
            }
        }
    }

    void JumpSFX()
    {
        int randoJump = Random.Range(0, jumpSFXNum);
        sfxManager.audioSource[0].clip = sfxManager.jumpSFX[randoJump];
        sfxManager.audioSource[0].Play();
    }

    public void BonkSFX()
    {
        sfxManager.audioSource[1].clip = sfxManager.bonkSFX;
        sfxManager.audioSource[1].Play();
    }

    void CheckPointSFX()
    {
        sfxManager.audioSource[2].clip = sfxManager.checkPointSFX;
        sfxManager.audioSource[2].Play();
    }

    public void SlideSFX()
    {
        sfxManager.audioSource[4].clip = sfxManager.slideSFX;
        sfxManager.audioSource[4].Play();
    }

    void PunchSFX()
    {
        sfxManager.audioSource[5].clip = sfxManager.punchSFX;
        sfxManager.audioSource[5].Play();
    }

    void DrinkSFX()
    {
        sfxManager.audioSource[6].clip = sfxManager.drinkSFX;
        sfxManager.audioSource[6].Play();
    }

    void ThrowSFX()
    {
        sfxManager.audioSource[7].clip = sfxManager.throwSFX;
        sfxManager.audioSource[7].Play();
    }

    void PickupBottleSFX()
    {
        sfxManager.audioSource[8].clip = sfxManager.bottlePickupSFX;
        sfxManager.audioSource[8].Play();
    }

    IEnumerator TxtActivator()
    {
        textPrompt.gameObject.SetActive(true);

        yield return new WaitForSeconds(5f);

        textPrompt.gameObject.SetActive(false);
    }

    IEnumerator EndTitleCard()
    {
        yield return new WaitForSeconds(2f);

        endScreen.SetActive(true);

        yield return new WaitForSeconds(2f);
    }

    IEnumerator ResetThrow()
    {
        yield return new WaitForSeconds(1f);

        readyThrow = true;
    }

    void PauseGame()
    {
        foreach (PlatformMover plats in startPlatforms)
        {
            plats.PauseGame();
        }
        foreach (EnemyBehaviour bads in enemies)
        {
            bads.StopMoving();
        }
        enemyActivation.SetActive(false);
    }

    //This happens when you touch a thing
    public void Dead()
    {
        if (!dead)
        {
            anim.SetBool("isDead", true);
            PauseGame();
            dead = true;
            isSliding = false;
            DecreaseBottles();
        }
    }

    public void PunchEventStart()
    {
        colPunch.SetActive(true);
        isPunching = true;
    }

    public void PunchEventEnd()
    {
        colPunch.SetActive(false);
        isPunching = false;
    }

    public void ActivateDeathTitleCard()
    {
        textPrompt.gameObject.SetActive(false);
        deathText.SetActive(true);
    }

    //This is the function that puts the game in motion
    void StartRunning()
    {
        anim.SetBool("isIdle", false);
        anim.SetBool("isRunning", true);
        gameStarted = true;
        enemyActivation.SetActive(true);

        foreach (PlatformMover plats in startPlatforms)
        {
            plats.ResumeGame();
        }
    }

    //The allmighty collision detector, if you hit something, you are dead!
    void OnCollisionEnter2D(Collision2D col)
    {
        EnemyBehaviour enemy = col.gameObject.GetComponent<EnemyBehaviour>();
        ShooterBehaviour shooter = col.gameObject.GetComponent<ShooterBehaviour>();
        BulletBehaviour bullet = col.gameObject.GetComponent<BulletBehaviour>();

        if (enemy || shooter || bullet) //Makes sure you don't die when you touch the ground
        {
            Dead();
        }
    }

    //The even more mighty trigger detector, if this hits something, a whole slew of things happen!
    void OnTriggerEnter2D(Collider2D collider)
    {
        EnemyBehaviour enemy = collider.gameObject.GetComponent<EnemyBehaviour>();
        ShooterBehaviour shooter = collider.gameObject.GetComponent<ShooterBehaviour>();
        BulletBehaviour bullet = collider.gameObject.GetComponent<BulletBehaviour>();

        if ((enemy || bullet || shooter) && boozedUp) //Destroys ALL obstacles if you're boozed up
        {
            if (enemy)
            {
                enemy.DeathAnim();
            }
            else if (shooter)
            {
                shooter.DeathAnim();
            }

            collider.gameObject.SetActive(false);


        }
        else if (shooter && !boozedUp && isPunching) //Destroys only shooters when punching
        {
            shooter.DeathAnim();
            collider.gameObject.SetActive(false);

        }

        if (collider.tag == "PitStop") //Sets the checkpoint if a pitstop is touched
        {
            SetCheckPoint();
            CheckPointSFX();
        }

        if (collider.tag == "Booze") //adds a Bottle of booze if you touch one
        {
            IncreaseBottles();
            PickupBottleSFX();
            collider.gameObject.SetActive(false);
        }

        if (collider.tag == "End")
        {
            EndSequence();
        }
    }

    void EndSequence()
    {
        Debug.Log("End sequence called, this should only show once per stage");
        anim.SetBool("isRunning", false);

        gameEnd = true;

        if (bottles < minBottleReq)
        {
            isWin = false;
            anim.SetTrigger("isSad");
            endScreenTxt.text = "You big palooka!\nYou only got " + bottles.ToString() + " and you needed " + minBottleReq.ToString() + "!\n Try again!";
            StartCoroutine(EndTitleCard());
        }
        else
        {
            isWin = true;
            anim.SetTrigger("isHappy");
            endScreenTxt.text = "Attaboy!\nYou brought " + bottles.ToString() + " more\n bottles for the speakeasy!\nYou're sitting pretty!";
            scoreMaster.AddToCurrentScore(bottles);
            StartCoroutine(EndTitleCard());
        }

        foreach (PlatformMover plats in startPlatforms)
        {
            plats.PauseGame();
        }
        foreach (EnemyBehaviour bads in enemies)
        {
            bads.StopMoving();
        }

        //TODO: Put the Iris transition here
        //TODO: Call LevelManager to change the level
    }

    //Drink up, be merry, and also invincible for a couple of seconds (pits don't care if you're drunk)
    void ActivateBoozePower()
    {
        if (Input.GetButtonDown("BoozeUp"))
        {
            if (!boozedUp && bottles >= 10)
            {
                DrinkSFX();
                bottles -= 10;
                UpdateBottleCountDisplay();
                boozedUp = true;
                colBoozePower.SetActive(true);
            }
            else if (bottles < 10)
            {
                textPrompt.text = "You're missing some hooch, fella!";
                StartCoroutine(TxtActivator());
            }
        }
    }

    //You sober up real quick, and you lose your invincibility
    void DeactivateBoozePower()
    {
        boozedUp = false;
        colBoozePower.SetActive(false);
        boozeTimer = initBoozeTimer;
    }

    //Puts everything back where it was, all neat and tidy
    void ResetEverything()
    {
        foreach (PlatformMover plats in startPlatforms)
        {
            plats.ResetPos();
            plats.gameObject.SetActive(true);
        }
        foreach (EnemyBehaviour bads in enemies)
        {
            bads.ResetPosition();
            bads.gameObject.SetActive(true);
        }

        foreach (BottlePickup collBottles in bottleCollectibles)
        {
            collBottles.gameObject.SetActive(true);
        }

        dead = false;
        deathText.SetActive(false);
        Initialize();
    }

    //Changes the running collider to the slide collider, also makes you look real cool
    void Slide()
    {
        if (Input.GetButton("Slide"))
        {
            anim.SetBool("isSliding", true);
            colRunning.SetActive(false);
            colSliding.SetActive(true);
            colTriggerRun.enabled = false;
            colTriggerSlide.enabled = true;
            isSliding = true;

        }
        else if (Input.GetButtonUp("Slide"))
        {
            anim.SetBool("isSliding", false);
            colRunning.SetActive(true);
            colSliding.SetActive(false);
            colTriggerRun.enabled = true;
            colTriggerSlide.enabled = false;
            isSliding = false;
        }
    }

    //Gets you really high
    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            //You can't jump while you're already jumping, dummy!
            if (isGrounded)
            {
                anim.SetTrigger("isJumping");
                JumpSFX();
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }
    }

    //Increases your booze count
    void IncreaseBottles()
    {

        bottles++;
        UpdateBottleCountDisplay();

    }

    //Remove bottles when you're hit
    void DecreaseBottles()
    {
        if (bottles >= 5)
        {
            bottles -= 5;
            UpdateBottleCountDisplay();
        }
        else
        {
            bottles = 0;
            UpdateBottleCountDisplay();
        }
    }

    //Throws a bottle
    void Throw()
    {
        if (Input.GetButtonDown("Throw")) //Set to something better
        {
            if (!isSliding && readyThrow)
            {
                if (bottles > 0)
                {
                    anim.SetTrigger("isThrowing");
                    ThrowSFX();
                    GameObject bottleProj = Instantiate(bottleThrown, throwingArm.position, Quaternion.identity) as GameObject;
                    bottles--;
                    UpdateBottleCountDisplay();
                    readyThrow = false;
                    Destroy(bottleProj, 3f);
                    StartCoroutine(ResetThrow());
                }
                else
                {
                    textPrompt.text = "You're outta moonshine, ya goon!";
                    StartCoroutine(TxtActivator());
                }
            }
        }

    }

    //Punchs a human enemy (?)
    void Punch()
    {
        if (Input.GetButtonDown("Punch")) //Set to something better
        {
            anim.SetTrigger("isPunching");
            PunchSFX();
        }
    }

    //Updates your booze count, white text if in normal range, green if above 24 and red if below...5?
    void UpdateBottleCountDisplay()
    {
        if (bottles <= minBottleReq)
        {
            bottleCountText.color = new Color32(200, 0, 0, 255);
        }
        else if (bottles <= 24)
        {
            bottleCountText.color = Color.white;
        }
        else if (bottles > 24)
        {
            bottleCountText.color = new Color32(114, 180, 80, 255);
        }

        bottleCountText.text = bottles.ToString();

    }

    void SetCheckPoint()
    {
        foreach (PlatformMover plats in startPlatforms)
        {
            Vector3 newPos = plats.GetPlatPosition();
            plats.SetPlatPosition(newPos - new Vector3(7, 0, 0));
        }
        foreach (EnemyBehaviour bads in enemies)
        {
            Vector3 newPos = bads.GetPosition();
            bads.SetPosition(newPos);
        }

        textPrompt.text = "Checkpoint reached!\n Everything's Jake!";
        StartCoroutine(TxtActivator());

    }

    //Sets all the colliders, animations, positions and conditions for the player back to normal.
    void Initialize()
    {
        colRunning.SetActive(true);
        colBoozePower.SetActive(false);
        colSliding.SetActive(false);
        colPunch.SetActive(false);
        colTriggerRun.enabled = true;
        colTriggerSlide.enabled = false;
        gameStarted = false;
        gameEnd = false;
        boozedUp = false;
        dead = false;
        transform.position = initPos;

        anim.SetBool("isDead", false);
        anim.SetBool("isIdle", true);
        anim.SetBool("isRunning", false);
        anim.SetBool("isSliding", false);
    }
}
