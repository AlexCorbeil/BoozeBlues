using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScoreMaster : MonoBehaviour
{
    public static ScoreMaster instance = null;


    static int currentScore;
    static int highScore;

    public Text currentScoreDisplay;
    public Text highScoreDisplay;



    const string PLAYER_PREFS_HIGHSCORE_KEY = "BoozeBlues_Highscore";

    // TODO: Find appropriate place to reset score, main menu?

    //Singleton pattern
    void Awake()
    {



    }

    void Start()
    {
        UpdateCurrentScoreDisplay();
        UpdateHighScoreDisplay();
    }

    public void UpdateHighScoreDisplay()
    {
        if (highScoreDisplay != null)
        {
            highScoreDisplay.text = GetHighScore().ToString();
        }
    }
    public void UpdateCurrentScoreDisplay()
    {
        if (currentScoreDisplay != null)
        {
            currentScoreDisplay.text = currentScore.ToString();
        }
    }
    public void ResetCurrentScore()
    {
        currentScore = 0;

    }
    public void AddToCurrentScore(int bottles)
    {
        currentScore += bottles;
        CheckAndSetHighScore(currentScore);
        Debug.Log("Got bottle count of " + bottles + "Current score is now" + currentScore);

    }


    public void CheckAndSetHighScore(int score)
    {
        if (score > GetHighScore())
        {

            PlayerPrefs.SetInt(PLAYER_PREFS_HIGHSCORE_KEY, score);
        }

    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public int GetHighScore()
    {
        return PlayerPrefs.GetInt(PLAYER_PREFS_HIGHSCORE_KEY);

    }





}

