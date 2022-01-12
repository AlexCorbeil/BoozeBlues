using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public float startSpeed = 10; //Initial speed of the enemy, set to 0 for stationary baddies
    public bool isActive;

    [SerializeField]
    protected GameObject killParticles;

    protected float speed;
    protected Rigidbody2D rb;
    protected Vector3 initialPosition;

    protected virtual void Start()
    {
        isActive = false;
        speed = startSpeed;
        rb = GetComponent<Rigidbody2D>();
        initialPosition = transform.localPosition;
    }

    void FixedUpdate()
    {
        if(!isActive)
        {
            speed = 0;
        }
        else
        {
            speed = startSpeed;
        }

        rb.velocity = new Vector2(-speed, rb.velocity.y);
    }

    public void StartMoving()
    {
        isActive = true;
    }

    public void StopMoving()
    {
        isActive = false;
    }

    public virtual void ResetPosition()
    {
        StopMoving();
        transform.localPosition = initialPosition;
    }

    public virtual Vector3 GetPosition()
    {
        return transform.localPosition;
    }

    public virtual void SetPosition(Vector3 pos)
    {
        initialPosition = pos;
    }

    public virtual void DeathAnim()
    {
        GameObject dustCloud = Instantiate(killParticles, transform.position, Quaternion.identity) as GameObject;
        Destroy(dustCloud, 2f);
    }
}
