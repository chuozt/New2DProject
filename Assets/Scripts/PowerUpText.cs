using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpText : Singleton<PowerUpText>
{
    public Text textToDisplay;
    public float wordDelay = 0.5f; // Delay between displaying each word

    public void ShowPowerUpText(string text)
    {
        StopAllCoroutines();
        StartCoroutine(DisplayTextWordByWord(text));
    }

    IEnumerator DisplayTextWordByWord(string text)
    {
        textToDisplay.enabled = true;
        textToDisplay.text = string.Empty;

        char[] words = text.ToCharArray();

        for (int i = 0; i < words.Length; i++)
        {
            textToDisplay.text += words[i];

            // Wait for wordDelay seconds before displaying the next word
            yield return new WaitForSeconds(wordDelay);
        }

        //Wait 3 secs before toggle off the text
        yield return new WaitForSeconds(3);
        textToDisplay.enabled = false;
    }
}
