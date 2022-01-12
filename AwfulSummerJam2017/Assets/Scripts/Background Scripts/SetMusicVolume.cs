using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SetMusicVolume : MonoBehaviour
{

    public Slider slider;
    MusicManager musicManager;

    void Start()
    {
        musicManager = FindObjectOfType<MusicManager>();
        slider.value = musicManager.GetVolume();
    }

    void Update()
    {
        musicManager.SetVolume(slider.value);
    }
}
