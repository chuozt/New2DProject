using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System;

public class UIManagerPauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuPanel;
    [SerializeField] GameObject audioSettingsPanel;
    public AudioClip sfxClick;
    CustomInput input;

    public static event Action onPause;
    public static event Action onUnPause;

    void Awake()
    {
        input = new CustomInput();
        pauseMenuPanel.SetActive(false);
        audioSettingsPanel.SetActive(false);
    }

    void OnEnable()
    {
        input.Enable();
        input.UI.Escape.performed += OnEscape;
    }

    void OnDisable()
    {
        input.Disable();
        input.UI.Escape.performed -= OnEscape;
    }

    void OnEscape(InputAction.CallbackContext context)
    {
        if(!pauseMenuPanel.activeInHierarchy)
            ToggleOnPauseMenu();
        else
            ButtonResume();
    }

    public void ToggleOnPauseMenu()
    {
        AudioManager.Instance.PlaySFX(sfxClick);
        onPause?.Invoke();
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0;
    }

    void ToggleOffPauseMenu()
    {
        AudioManager.Instance.PlaySFX(sfxClick);
        onUnPause?.Invoke();
        pauseMenuPanel.SetActive(false);
    }

    void ToggleOnAudioSettings()
    {
        AudioManager.Instance.PlaySFX(sfxClick);
        audioSettingsPanel.SetActive(true);
    }

    void ToggleOffAudioSettings()
    {
        AudioManager.Instance.PlaySFX(sfxClick);
        audioSettingsPanel.SetActive(false);
    }

    public void ButtonResume()
    {
        AudioManager.Instance.PlaySFX(sfxClick);
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void ButtonSettings()
    {
        AudioManager.Instance.PlaySFX(sfxClick);
        ToggleOnAudioSettings();
        ToggleOffPauseMenu();
    }

    public void BackButton()
    {
        AudioManager.Instance.PlaySFX(sfxClick);
        ToggleOnPauseMenu();
        ToggleOffAudioSettings();
    }

    public void ButtonBackToMainMenu()
    {
        AudioManager.Instance.PlaySFX(sfxClick);
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenuScene");
    }
}
