using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    static public float seconds = 0f;
    static public int minutes = 0;
    void Update()
    {
        // only adds to timer if player hasn't already won,
        // this is so time waiting for the score screen isn't counted
        if (!CheckForWin.hasWon) {
            seconds += Time.deltaTime;
            if (seconds > 60) {
                seconds = seconds % Time.deltaTime;
                minutes++;
            }
        }
    }
}
