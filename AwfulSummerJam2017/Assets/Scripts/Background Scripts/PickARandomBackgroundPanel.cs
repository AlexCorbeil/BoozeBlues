using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickARandomBackgroundPanel : MonoBehaviour
{

    /*This should be placed on the platform pool for a level. */


    SpriteRenderer spriteRender;
    public Sprite[] panelArray; // assign background panels from Sprites\BG folder

    // Put this on platform pool for a level   


    void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Platform"))
            {
                AssignBackground(child.Find("BG"));
            }
        }

    }
    void AssignBackground(Transform transform)
    {
        spriteRender = transform.GetComponent<SpriteRenderer>();
        spriteRender.sprite = panelArray[Random.Range(0, panelArray.Length)];
    }


}
