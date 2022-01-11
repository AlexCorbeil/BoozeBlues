using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class FadeIn : MonoBehaviour 
{
    private LevelManager lvlmanager;
    private Image panel;


    void Start()
    {
        panel = GetComponent<Image>();
        lvlmanager = GameObject.Find("LevelManager").GetComponent<LevelManager>();

        StartCoroutine(FadeInImage());

    }

    void Update()
    {
        if(Input.GetButtonDown("Submit"))
        {
            Continue();
        }
    }

    public void Continue()
    {
        StartCoroutine(FadeOutImage());
    }

    void StartPlaying()
    {
        lvlmanager.LoadLevel("Level 1");
    }

    IEnumerator FadeInImage()
    {
        for(float i = 1; i >= 0; i -= Time.deltaTime)
        {
            panel.color = new Color(0, 0, 0, i);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(5f);

    }

    IEnumerator FadeOutImage()
    {
        for(float i = 0; i <= 1; i += Time.deltaTime)
        {
            panel.color = new Color(0, 0, 0, i);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(2f);

        StartPlaying();

    }
}
