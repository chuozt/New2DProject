using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private Text timeText;
    [SerializeField] private Text finalTimeText;
    float elapsedTime = 0;
    bool isEndedGame = false;

    void OnEnable() => TimelineTrigger.onEndGame += SetEndGame;

    void Update()
    {
        if(isEndedGame)
            return;

        elapsedTime += Time.deltaTime;
        timeText.text = "Time: " + FormatTime(elapsedTime);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void SetEndGame()
    {
        finalTimeText.text = "You have finished the game in\n" + FormatTime(elapsedTime);
        isEndedGame = true;
    }
}
