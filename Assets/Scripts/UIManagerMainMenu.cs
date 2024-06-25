using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManagerMainMenu : MonoBehaviour
{
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject audioSettingsPanel;
    [SerializeField] AudioClip sfxClick;
    [SerializeField] Image blackScreenImage;
    
    bool isPressedStart = false;

    void Awake()
    {
        mainMenuPanel.SetActive(true);
        audioSettingsPanel.SetActive(false);
    }

    //MAIN MENU
    public void ButtonStart()
    {
        if(isPressedStart)
            return;
        
        AudioManager.Instance.PlaySFX(sfxClick);
        isPressedStart = true;
        StartCoroutine(FadeInImage(2));
    }

    public void ButtonSettings()
    {
        AudioManager.Instance.PlaySFX(sfxClick);
        mainMenuPanel.SetActive(false);
        audioSettingsPanel.SetActive(true);
    }

    public void ButtonExit()
    {
        AudioManager.Instance.PlaySFX(sfxClick);
        Application.Quit();
    }

    public void ButtonBack()
    {
        AudioManager.Instance.PlaySFX(sfxClick);
        audioSettingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    IEnumerator FadeInImage(float duration)
    {
        float elapsedTime = 0f;
        Color initialColor = blackScreenImage.color;
        initialColor.a = 0f;
        blackScreenImage.color = initialColor;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / duration);
            Color newColor = blackScreenImage.color;
            newColor.a = alpha;
            blackScreenImage.color = newColor;
            yield return null;
        }

        // Ensure the image is fully visible at the end of the fade
        Color finalColor = blackScreenImage.color;
        finalColor.a = 1f;
        blackScreenImage.color = finalColor;
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
}
