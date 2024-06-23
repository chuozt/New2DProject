using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue", fileName = "Dialogue")]
public class SO_Dialogues : ScriptableObject
{
    [field:SerializeField] public Sprite MainCharacterImage { get; set; }
    [field:SerializeField] public Sprite SideCharacterImage { get; set; }
    
    [field:SerializeField] public List<DialogueStruct> DialogueStructs { get; set; }
}

[System.Serializable]
public struct DialogueStruct
{
    [field:SerializeField] public int CharacterIndex { get; set; }
    [field:SerializeField, TextArea(1,4)] public string DialogueText { get; set; }
}
