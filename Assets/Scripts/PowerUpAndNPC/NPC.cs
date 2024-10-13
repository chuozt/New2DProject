using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    [SerializeField] private SO_Dialogues SO_Dialogue;
    [SerializeField] GameObject powerUp;
    [SerializeField] private SpriteRenderer arrowSR;
    private bool isTriggered = false;

    void Awake()
    {
        powerUp.SetActive(false);
        arrowSR.enabled = false;
    }

    void OnEnable() => DialogueManager.onEndDialogue += EnablePowerUp;

    void OnDisable() => DialogueManager.onEndDialogue -= EnablePowerUp;

    void EnablePowerUp()
    {
        if(isTriggered && powerUp != null)
            powerUp.SetActive(true);
    }

    public void Interact()
    {
        if(!isTriggered)
        {
            isTriggered = true;
            DialogueManager.Instance.StartDialogue(SO_Dialogue);
        }
        else
            PlayerScript.Instance.SetIsInteractingFlag(false);
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
