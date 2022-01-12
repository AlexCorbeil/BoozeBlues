using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public float speed;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
       rb.velocity = new Vector2(-speed, rb.velocity.y);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        PlayerBehaviour player = col.gameObject.GetComponent<PlayerBehaviour>();

        if(player)
        {
            Destroy(gameObject);
        }
    }
}
