using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{
    public float jumpForce;
    public LayerMask whatIsGround;
    public int initBottles = 24;
    public float initBoozeTimer = 5f;
    public int minBottleReq = 10;

    private bool isGrounded;
    private bool isWin = false;
    private Vector3 initPos;
    private float boozeTimer;
    private int bottles;
    private Rigidbody2D rb;
    [SerializeField] private CapsuleCollider2D runCollider;
    [SerializeField] private CapsuleCollider2D slideCollider;
    private Animator anim;
    private bool gameStarted; 
    private bool boozedUp;
    private bool isDead = false;
    private bool isSliding = false;
    private bool isPunching = false;
    private bool readyThrow = true;
    private bool gameEnd = false;
    private SFXManager sfxManager;
    private int jumpSFXNum;
    private PlatformMover[] startPlatforms;
    private EnemyBehaviour[] enemies;
    private BottlePickup[] bottleCollectibles;
    private LevelManager levelMng;
    private ScoreMaster scoreMaster;

    [SerializeField]
    private GameObject bottleThrown;
    [SerializeField]
    private Transform throwingArm;
    [SerializeField]
    private GameObject colRunning;
    [SerializeField]
    private GameObject colSliding;
    [SerializeField]
    private GameObject colBoozePower;
    [SerializeField]
    private GameObject colPunch;
    [SerializeField]
    private CapsuleCollider2D colTriggerRun;
    [SerializeField]
    private CapsuleCollider2D colTriggerSlide;
    [SerializeField]
    private Text bottleCountText;
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

        boozeTimer = initBoozeTimer;
        bottles = initBottles;
        initPos = transform.position;

        UpdateBottleCountDisplay();
        Initialize();
    }

    void FixedUpdate()
    {
        if (!isSliding)
        {
            isGrounded = Physics2D.IsTouchingLayers(runCollider, whatIsGround);
        }
        else
        {
            isGrounded = Physics2D.IsTouchingLayers(slideCollider, whatIsGround);
        }

        anim.SetBool("isGrounded", isGrounded);

    }

    void Update()
    {

        if (!gameStarted)
        {
            PauseGame();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                startText.SetActive(false);
                StartRunning();
            }
        }
        else if (isDead)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ResetEverything();
            }
        }
        else if (!gameEnd)
        {
            Jump();
            Slide();
            ActivateBoozePower();
            Throw();
            Punch();

            
            if (boozedUp)
            {
                boozeTimer -= Time.deltaTime;

                if (boozeTimer <= 0)
                {
                    DeactivateBoozePower();
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

    public void Dead()
    {
        if (!isDead)
        {
            anim.SetBool("isDead", true);
            PauseGame();
            isDead = true;
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

    void OnCollisionEnter2D(Collision2D col)
    {
        EnemyBehaviour enemy = col.gameObject.GetComponent<EnemyBehaviour>();
        ShooterBehaviour shooter = col.gameObject.GetComponent<ShooterBehaviour>();
        BulletBehaviour bullet = col.gameObject.GetComponent<BulletBehaviour>();

        if (enemy || shooter || bullet)
        {
            Dead();
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        EnemyBehaviour enemy = collider.gameObject.GetComponent<EnemyBehaviour>();
        ShooterBehaviour shooter = collider.gameObject.GetComponent<ShooterBehaviour>();
        BulletBehaviour bullet = collider.gameObject.GetComponent<BulletBehaviour>();

        if ((enemy || bullet || shooter) && boozedUp)
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
        else if (shooter && !boozedUp && isPunching)
        {
            shooter.DeathAnim();
            collider.gameObject.SetActive(false);

        }

        if (collider.tag == "PitStop")
        {
            SetCheckPoint();
            CheckPointSFX();
        }

        if (collider.tag == "Booze")
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
    }

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

    void DeactivateBoozePower()
    {
        boozedUp = false;
        colBoozePower.SetActive(false);
        boozeTimer = initBoozeTimer;
    }

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

        isDead = false;
        deathText.SetActive(false);
        Initialize();
    }

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

    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                anim.SetTrigger("isJumping");
                JumpSFX();
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }
    }

    void IncreaseBottles()
    {

        bottles++;
        UpdateBottleCountDisplay();

    }

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

    void Throw()
    {
        if (Input.GetButtonDown("Throw"))
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

    void Punch()
    {
        if (Input.GetButtonDown("Punch"))
        {
            anim.SetTrigger("isPunching");
            PunchSFX();
        }
    }

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
        isDead = false;
        transform.position = initPos;

        anim.SetBool("isDead", false);
        anim.SetBool("isIdle", true);
        anim.SetBool("isRunning", false);
        anim.SetBool("isSliding", false);
    }
}
