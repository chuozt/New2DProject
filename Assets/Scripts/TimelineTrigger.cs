using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System;

public class TimelineTrigger : MonoBehaviour
{
    [SerializeField] private PlayableDirector playableDirector;

    public static event Action onEndGame;

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
        {
            playableDirector.Play();
            PlayerScript.Instance.SetCanMoveFlag(false);
            onEndGame?.Invoke();
        }
    }
}
