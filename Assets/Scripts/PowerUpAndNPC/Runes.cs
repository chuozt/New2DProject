using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runes : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioClip sfxPickUp;

    public void Interact()
    {
        AudioManager.Instance.PlaySFX(sfxPickUp);
        RunesText.Instance.UpdateRuneText();
        PlayerScript.Instance.SetIsInteractingFlag(false);
        gameObject.SetActive(false);
    }
}
