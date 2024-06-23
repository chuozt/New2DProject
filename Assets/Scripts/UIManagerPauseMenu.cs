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
        onPause?.Invoke();
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0;
    }

    void ToggleOffPauseMenu()
    {
        onUnPause?.Invoke();
        pauseMenuPanel.SetActive(false);
    }

    void ToggleOnAudioSettings()
    {
        audioSettingsPanel.SetActive(true);
    }

    void ToggleOffAudioSettings()
    {
        audioSettingsPanel.SetActive(false);
    }

    public void ButtonResume()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void ButtonSettings()
    {
        ToggleOnAudioSettings();
        ToggleOffPauseMenu();
    }

    public void BackButton()
    {
        ToggleOnPauseMenu();
        ToggleOffAudioSettings();
    }

    public void ButtonBackToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenuScene");
    }
}
