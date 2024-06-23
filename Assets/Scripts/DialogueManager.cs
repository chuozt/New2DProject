using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Text dialogueText;
    [SerializeField] private float textSpeed;
    [SerializeField] private Image characterImage;

    private SO_Dialogues SO_Dialogue;
    private List<string> lines = new List<string>();
    private int index;

    public static event Action onEndDialogue;

    void Start()
    {
        dialogueText.text = string.Empty;
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(SO_Dialogues so_dialogue)
    {
        //Set the player as in dialogue
        PlayerScript.Instance.SetIsInDialogueFlag(true);
        dialoguePanel.SetActive(true);

        SO_Dialogue = so_dialogue;

        //Get all voice lines
        foreach(var dialogueStruct in SO_Dialogue.DialogueStructs)
            lines.Add(dialogueStruct.DialogueText);

        index = 0;
        SetCharacterComponents(index);
        StartCoroutine(TypeLine());
    }

    void SetCharacterComponents(int index)
    {
        //Set images and name texts for the respective character
        if(SO_Dialogue.DialogueStructs[index].CharacterIndex == 0)
            characterImage.sprite = SO_Dialogue.SideCharacterImage;
        else
            characterImage.sprite = SO_Dialogue.MainCharacterImage;
    }

    public void NextButton()
    {
        //If the line is finish, then go to the next line
        if (dialogueText.text == lines[index])
            NextLine();
        else //Else finish that line first
        {
            StopAllCoroutines();
            dialogueText.text = lines[index];
        }
    }
    
    IEnumerator TypeLine()
    {
        //Expose line word-by-word
        foreach (char c in lines[index].ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        //If this was not the last line, show next line
        if (index < lines.Count - 1)
        {
            index++;
            dialogueText.text = string.Empty;
            SetCharacterComponents(index);
            StartCoroutine(TypeLine());
        }
        else //Else toggle off related components
        {
            PlayerScript.Instance.SetIsInteractingFlag(false);
            PlayerScript.Instance.SetIsInDialogueFlag(false);
            onEndDialogue?.Invoke();
            dialoguePanel.SetActive(false);
        }
    }
}
