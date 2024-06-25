using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunesText : Singleton<RunesText>
{
    Text runesText;
    int count = 0;

    void Start()
    {
        runesText = GetComponent<Text>();
        runesText.color = new Color(runesText.color.r, runesText.color.g, runesText.color.b, 0);
    }

    public void UpdateRuneText()
    {
        runesText.text = string.Empty;
        count++;
        runesText.text = "Hidden Runes Collected: " + count + "/5...";
        StartCoroutine(FadeRunesText(4));
    }

    IEnumerator FadeRunesText(float duration)
    {
        float halfDuration = duration / 2f;

        // Fade In
        for (float t = 0.01f; t < halfDuration; t += Time.deltaTime)
        {
            Color newColor = runesText.color;
            newColor.a = Mathf.Lerp(0, 1, t / halfDuration);
            runesText.color = newColor;
            yield return null;
        }

        // Ensure fully visible
        Color finalColor = runesText.color;
        finalColor.a = 1;
        runesText.color = finalColor;

        // Wait for a brief moment if needed
        yield return new WaitForSeconds(0.1f);

        // Fade Out
        for (float t = 0.01f; t < halfDuration; t += Time.deltaTime)
        {
            Color newColor = runesText.color;
            newColor.a = Mathf.Lerp(1, 0, t / halfDuration);
            runesText.color = newColor;
            yield return null;
        }

        // Ensure fully invisible
        finalColor = runesText.color;
        finalColor.a = 0;
        runesText.color = finalColor;
    }
}
