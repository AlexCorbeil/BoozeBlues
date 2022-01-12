using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelTransition : MonoBehaviour
{

    void OnTriggerEnter2d(Collider col)
    {
        if (col.tag == "Player")
        {
            Debug.Log("player crossed me, call level manager");
        }
    }
}
