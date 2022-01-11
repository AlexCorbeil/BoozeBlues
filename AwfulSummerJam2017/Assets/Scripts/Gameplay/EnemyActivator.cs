using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActivator : MonoBehaviour 
{
    //Activates the enemy if it's a mobile one once it touches this collider
    void OnTriggerEnter2D(Collider2D col)
    {
        EnemyBehaviour enemy = col.gameObject.GetComponent<EnemyBehaviour>();
        ShooterBehaviour shooter = col.gameObject.GetComponent<ShooterBehaviour>();

        if(enemy)
        {
            enemy.StartMoving();
        }

        if(shooter)
        {
            shooter.Shoot();
        }
    }
}
