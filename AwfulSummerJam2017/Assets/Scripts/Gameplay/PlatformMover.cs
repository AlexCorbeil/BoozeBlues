using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    public float startingSpeed = 10f;

    private float speed = 0f;
    private Vector3 initPos;

    void Start()
    {
        speed = 0f;
        initPos = transform.position;
    }

    void FixedUpdate()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }
	
    public void ResumeGame()
    {
        speed = startingSpeed;
    }

    public void PauseGame()
    {
        speed = 0f;
    }

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
