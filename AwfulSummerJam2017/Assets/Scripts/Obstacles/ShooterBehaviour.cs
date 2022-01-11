using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterBehaviour : EnemyBehaviour {

    public Transform gunBarrel; //Where the bullet will spawn from

    private Animator anim;
    private SFXManager sfxManager;
    [SerializeField]
    private GameObject bullet;

    //Runs the standard Start function from EnemyBehaviour
    //adds the animator and sfxManager for the shooter only
    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        sfxManager = GameObject.FindObjectOfType<SFXManager>();
    }

    //Starts the Shoot animation and sfx, then spawns a bullet from the gunBarrel's location
    public void Shoot()
    {
        anim.SetTrigger("isShooting");
        ShotSFX();
        GameObject shot = Instantiate(bullet, gunBarrel.position, Quaternion.identity) as GameObject;
        shot.transform.parent = transform; //This makes sure the bullet's clone is added as the shooter's child object
        Destroy(shot, 3f);
    }

    //Plays the shot sound effects, as it says on the tin.
    void ShotSFX()
    {
        sfxManager.audioSource[3].clip = sfxManager.shotSFX;
        sfxManager.audioSource[3].Play();
    }
	
}
