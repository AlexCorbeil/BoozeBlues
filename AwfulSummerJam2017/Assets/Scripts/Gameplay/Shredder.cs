using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shredder : MonoBehaviour 
{
    private PlatformPooler platPool;
    private EnemyBehaviour enemy;
    private PlayerBehaviour player;

    void OnTriggerEnter2D(Collider2D collider)
    {
        player = collider.gameObject.GetComponentInParent<PlayerBehaviour>();
        enemy = collider.gameObject.GetComponent<EnemyBehaviour>();
        platPool = collider.gameObject.GetComponentInParent<PlatformPooler>();

        if(enemy)
        {
            enemy.ResetPosition();    
        }

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
