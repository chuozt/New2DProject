using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runes : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioClip sfxPickUp;
    [SerializeField] private SpriteRenderer arrowSR;

    void Awake()
    {
        arrowSR.enabled = false;
    }

    public void Interact()
    {
        AudioManager.Instance.PlaySFX(sfxPickUp);
        RunesText.Instance.UpdateRuneText();
        PlayerScript.Instance.SetIsInteractingFlag(false);
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
            arrowSR.enabled = true;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
            arrowSR.enabled = false;
    }
}
