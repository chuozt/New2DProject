using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGameManager : MonoBehaviour
{
    [SerializeField] Image startBlackScreen;
    [SerializeField, TextArea] string startText;

    void Start()
    {
        if (startBlackScreen != null)
        {
            // Example usage
            StartCoroutine(FadeOutImage(2f)); // Start fading out over 2 seconds
        }
    }

    IEnumerator FadeOutImage(float duration)
    {
        float elapsedTime = 0f;
        Color initialColor = startBlackScreen.color;
        initialColor.a = 1f;
        startBlackScreen.color = initialColor;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1 - (elapsedTime / duration));
            Color newColor = startBlackScreen.color;
            newColor.a = alpha;
            startBlackScreen.color = newColor;
            yield return null;
        }

        // Ensure the image is fully invisible at the end of the fade
        Color finalColor = startBlackScreen.color;
        finalColor.a = 0f;
        startBlackScreen.color = finalColor;

        PowerUpText.Instance.ShowPowerUpText(startText);
    }
}
