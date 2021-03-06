using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawDropShadow : MonoBehaviour
{
    float dropShadowPosition;
    float shadowOffsetY;
    Vector3 parentBounds;
    SpriteRenderer spriteRender;
    int groundLayer;
    float rayDistance = 10;

    void Start()
    {
        groundLayer = LayerMask.GetMask("Ground");
    }

    public void UpdateShadowHeightAndScale()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.parent.position, Vector2.down, rayDistance, groundLayer);
        if (hit)
        {
            shadowOffsetY = hit.collider.bounds.max.y;
            float scaleAmount = 30 / hit.distance;
            transform.localScale = new Vector2(scaleAmount, scaleAmount);
        }
    }

    void Update()
    {
        UpdateShadowHeightAndScale();
        transform.position = new Vector2(transform.position.x, shadowOffsetY);
    }
}
