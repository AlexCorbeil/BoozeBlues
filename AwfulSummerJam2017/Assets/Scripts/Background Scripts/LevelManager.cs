using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance = null;
    static string currentLevelName;


    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
         
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadSceneAsync(levelName);

    }
    public void LoadLevel(int sceneIndex)
    {
        SceneManager.LoadSceneAsync(sceneIndex);

    }


    public void QuitGame()
    {
        Application.Quit();
    }


}
