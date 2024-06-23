using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer mainGameAudioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    public void SetMusicVolume() => mainGameAudioMixer.SetFloat("Music", musicSlider.value);
    
    public void SetSFXVolume() => mainGameAudioMixer.SetFloat("SFX", sfxSlider.value);
}
