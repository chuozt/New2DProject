using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManagerMainMenu : MonoBehaviour
{
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject audioSettingsPanel;

    void Awake()
    {
        mainMenuPanel.SetActive(true);
        audioSettingsPanel.SetActive(false);
    }

    //MAIN MENU
    public void ButtonStart()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void ButtonSettings()
    {
        mainMenuPanel.SetActive(false);
        audioSettingsPanel.SetActive(true);
    }

    public void ButtonExit()
    {
        Application.Quit();
    }

    public void ButtonBack()
    {
        audioSettingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
