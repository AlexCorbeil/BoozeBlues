using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    public float startingSpeed = 10f; //The platform's starting speed

    private float speed = 0f; //The platform's current speed
    private Vector3 initPos; //The platform's initial position

    void Start()
    {
        //Sets all the initial info
        speed = 0f;
        initPos = transform.position;
    }

    void FixedUpdate()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime); //Moves the platform left
    }
	
    //Starts moving the platforms
    public void ResumeGame()
    {
        speed = startingSpeed;
    }

    //Pauses the platforms
    public void PauseGame()
    {
        speed = 0f;
    }

    //Resets the Platforms
    public void ResetPos()
    {
        transform.position = initPos;
    }

    public Vector3 GetPlatPosition()
    {
        return transform.position;
    }

    public void SetPlatPosition(Vector3 pos)
    {
        initPos = pos;
    }
}
