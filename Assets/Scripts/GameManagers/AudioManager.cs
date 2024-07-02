using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("- Audio Sources -")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    void OnEnable()
    {
        UIManagerPauseMenu.onPause += PauseAudio;
        UIManagerPauseMenu.onUnPause += UnPauseAudio;
    }

    void OnDisable()
    {
        UIManagerPauseMenu.onPause -= PauseAudio;
        UIManagerPauseMenu.onUnPause -= UnPauseAudio;
    }

    public void PlaySFX(AudioClip clip) => sfxSource.PlayOneShot(clip);

    public void PlayMusic(AudioClip clip) => musicSource.PlayOneShot(clip);

    public void StopSFX() => sfxSource.Stop();

    void PauseAudio()
    {
        musicSource.Pause();
        sfxSource.Pause();
    }

    void UnPauseAudio()
    {
        musicSource.UnPause();
        sfxSource.UnPause();
    }
}
