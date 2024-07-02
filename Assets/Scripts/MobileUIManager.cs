using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileUIManager : Singleton<MobileUIManager>
{
    [SerializeField] private List<Button> buttons = new List<Button>();
    public Transform player;
    public Camera mainCamera;
    public float fadeDuration = 1.0f;
    public float fadeAlpha = 0.25f;
    public Vector2 bottomRightAreaBuffer = new Vector2(200, 200);

    bool isFadingOut, isFadingIn;
    bool isFadedOut = false, isFadedIn = true;

    void OnEnable()
    {
        PlayerScript.onCloseToInteractable += ToggleOnButton;
        PlayerScript.onOutInteractable += ToggleOffButton;
    }

    void OnDisable()
    {
        PlayerScript.onCloseToInteractable -= ToggleOnButton;
        PlayerScript.onOutInteractable -= ToggleOffButton;
    }

    void Awake()
    {
        ToggleOffButton(IngameButton.Interact);
        ToggleOffButton(IngameButton.Float);
        ToggleOffButton(IngameButton.Dash);
    }

    void Update()
    {
        Vector2 playerScreenPosition = mainCamera.WorldToScreenPoint(player.position);
        if (IsInBottomRightArea(playerScreenPosition) && !isFadedOut)
            FadeOutAllButtons();
        else if(!IsInBottomRightArea(playerScreenPosition) && !isFadedIn)
            FadeInAllButtons();
    }

    bool IsInBottomRightArea(Vector2 screenPosition)
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        Debug.Log(screenPosition.x >= screenWidth/2 + bottomRightAreaBuffer.x && screenPosition.x <= screenWidth && 
               screenPosition.y >= 0 && screenPosition.y <= screenHeight/2 + bottomRightAreaBuffer.y);
               
        return screenPosition.x >= screenWidth/2 + bottomRightAreaBuffer.x && screenPosition.x <= screenWidth && 
               screenPosition.y >= 0 && screenPosition.y <= screenHeight/2 + bottomRightAreaBuffer.y;
    }

    public void FadeOutSingleButton(IngameButton button)
    {
        StartCoroutine(FadeOutSingleButtonCoroutine(button));
    }

    IEnumerator FadeOutSingleButtonCoroutine(IngameButton button)
    {
        Image image = buttons[(int)button].GetComponent<Image>();
        float tempAlpha = image.color.a;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            tempAlpha = Mathf.Lerp(tempAlpha, fadeAlpha, t / fadeDuration);
            image.color = new Color(image.color.r, image.color.g, image.color.b, tempAlpha);
            yield return null;
        }

        image.color = new Color(image.color.r, image.color.g, image.color.b, fadeAlpha);
        isFadedIn = false;
        isFadedOut = true;
    }

    public void FadeInSingleButton(IngameButton button)
    {
        StartCoroutine(FadeInSingleButtonCoroutine(button));
    }

    IEnumerator FadeInSingleButtonCoroutine(IngameButton button)
    {
        Image image = buttons[(int)button].GetComponent<Image>();
        float tempAlpha = fadeAlpha;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            tempAlpha = Mathf.Lerp(tempAlpha, 1, t / fadeDuration);
            image.color = new Color(image.color.r, image.color.g, image.color.b, tempAlpha);
            yield return null;
        }

        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        isFadedIn = true;
        isFadedOut = false;
    }

    public void FadeOutAllButtons()
    {
        for(int i = 0; i < buttons.Count; i++)
            StartCoroutine(FadeOutSingleButtonCoroutine((IngameButton)i));
    }

    public void FadeInAllButtons()
    {
        for(int i = 0; i < buttons.Count; i++)
            StartCoroutine(FadeInSingleButtonCoroutine((IngameButton)i));
    }

    public void ToggleOnButton(IngameButton button)
    {
        buttons[(int)button].GetComponent<Image>().enabled = true;
        buttons[(int)button].enabled = true;
    }

    public void ToggleOnAllButtons()
    {
        for(int i = 0; i < buttons.Count; i++)
            ToggleOnButton((IngameButton)i);
    }

    public void ToggleOffButton(IngameButton button)
    {
        buttons[(int)button].GetComponent<Image>().enabled = false;
        buttons[(int)button].enabled = false;
    }

    public void ToggleOffAllButtons()
    {
        for(int i = 0; i < buttons.Count; i++)
            ToggleOffButton((IngameButton)i);
    }

    public void OnTogglingFloatButton() => buttons[(int)IngameButton.Float].image.color = Color.yellow;

    public void OnTogglingOffFloatButton() => buttons[(int)IngameButton.Float].image.color = Color.white;
}

public enum IngameButton
{
    Jump = 0,
    Float,
    Dash,
    Interact
}