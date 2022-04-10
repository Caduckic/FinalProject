using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OutOfBoundsTimer : MonoBehaviour
{
    private float timer;
    private Text timerUI;
    // uses OnEnable as this helps to reset the timer
    void OnEnable() {
        timer = 6;
        timerUI = gameObject.GetComponent<Text>();
    }
    void Update() {
        // removes from timer using deltaTime
        timer -= Time.deltaTime;
        // displays timer cast to a string, F1 is used to cut off the float so its not overly long
        timerUI.text = timer.ToString("F1");
        // if the timer runs out then it will throw the player back to the start menu
        if (timer <= 0) {
            ChangeScene.NextScene("StartMenu");
        }
    }
}
