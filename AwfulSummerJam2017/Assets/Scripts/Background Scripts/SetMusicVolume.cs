using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SetMusicVolume : MonoBehaviour
{

    public Slider slider;
    MusicManager musicManager;
    // Use this for initialization
    void Start()
    {
        musicManager = FindObjectOfType<MusicManager>();
        slider.value = musicManager.GetVolume();
    }

    // Update is called once per frame



    void Update()
    {
        musicManager.SetVolume(slider.value);
    }
}
