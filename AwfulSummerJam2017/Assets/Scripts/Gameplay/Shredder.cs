using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shredder : MonoBehaviour 
{
    private PlatformPooler platPool; //The platform pooler, to bring back dead platforms
    private EnemyBehaviour enemy; //The enemy that touches the shredder
    private PlayerBehaviour player; //The player, of course

    //DA SHREDDER!!!
    void OnTriggerEnter2D(Collider2D collider)
    {
        //Grabs the components when touching the shredder
        player = collider.gameObject.GetComponentInParent<PlayerBehaviour>();
        enemy = collider.gameObject.GetComponent<EnemyBehaviour>();
        platPool = collider.gameObject.GetComponentInParent<PlatformPooler>();

        //If the object is an enemy, reset its position
        if(enemy)
        {
            enemy.ResetPosition();    
        }

        //If the object is the platform's ground, deactivate the entire platform and put it back in the pool
        if(collider.tag == "Ground")
        {

            collider.transform.parent.gameObject.SetActive(false);
            platPool.AddPlatformBack(collider.transform.parent.gameObject);

        }

        //If the object is the starting platform, simply deactivate it, THIS PLATFORM DOES NOT GO IN THE POOL!
        if(collider.tag == "StartingPlatform")
        {
            collider.transform.parent.gameObject.SetActive(false); 
        }

        //If the player falls down, he dies
        if(player)
        {
            player.Dead();
        }

        //If the platform is a PitStop clone, just delete it, since it's a copy of the one in the pool
        if(collider.tag == "PitStopPlatform")
        {
            collider.transform.parent.gameObject.SetActive(false);
        }
    }
}
