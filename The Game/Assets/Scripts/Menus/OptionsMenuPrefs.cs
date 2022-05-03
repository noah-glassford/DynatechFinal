using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JSAM;

public class OptionsMenuPrefs : MonoBehaviour
{
    //This script keeps track of the player prefs

    [SerializeField]
    private Slider MouseSensSlider;
    public Slider MasterVolumeSlider, MusicVolumeSlider, SFXVolumeSlider;

    public float MouseSens;
    public float MasterVolume = JSAM.AudioManager.GetMasterVolume();
    public float MusicVolume = JSAM.AudioManager.GetMusicVolume();
    public float SFXVolume = JSAM.AudioManager.GetSoundVolume();


    private void Awake()
    {
        if (!PlayerPrefs.HasKey("MouseSens"))
        {
            PlayerPrefs.SetFloat("MouseSens", 1f); //defaults sens to 1 if no value is set
        }

        MouseSens = PlayerPrefs.GetFloat("MouseSens");
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume");
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume");
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume");
        MasterVolumeSlider.value = JSAM.AudioManager.GetMasterVolume();
        MusicVolumeSlider.value = JSAM.AudioManager.GetMusicVolume();
        SFXVolumeSlider.value = JSAM.AudioManager.GetSoundVolume();
    }

    // Update is called once per frame
    void Update()
    {
        
        //MouseSensSlider.value = MouseSens;
    }

    public void SetMouseSensitivity()
    {
        MouseSens = MouseSensSlider.value;
        PlayerPrefs.SetFloat("MouseSens", MouseSens);
    }

    public void SetMasterVolume()
    {
        MasterVolume = MasterVolumeSlider.value;
        PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
        JSAM.AudioManager.SetMasterVolume(MasterVolume);
    }

    public void SetMusicVolume()
    {
        MusicVolume = MusicVolumeSlider.value;
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        JSAM.AudioManager.SetMusicVolume(MusicVolume);
    }

    public void SetSFXVolume()
    {
        SFXVolume = SFXVolumeSlider.value;
        PlayerPrefs.SetFloat("SFXVolume", SFXVolume);
        JSAM.AudioManager.SetSoundVolume(SFXVolume);
    }
}
