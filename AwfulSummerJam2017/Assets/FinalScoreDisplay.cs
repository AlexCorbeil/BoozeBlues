using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalScoreDisplay : MonoBehaviour
{
    ScoreMaster scoreMaster;
    Text finalScoreText;
    int finalScore;
    // Use this for initialization
    void Start()
    {
        finalScoreText = GetComponent<Text>();
        scoreMaster = GameObject.FindObjectOfType<ScoreMaster>();
        finalScore = scoreMaster.GetCurrentScore();

        finalScoreText.text = "Jimmy delivered " + finalScore + " bottles!";
        scoreMaster.ResetCurrentScore();


    }

    // Update is called once per frame
    void Update()
    {

    }
}
