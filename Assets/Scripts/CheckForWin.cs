using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CheckForWin : MonoBehaviour
{
    static public bool hasWon = false;
    private bool startedCoroutine = false;
    void Update() {
        // if it hasn't already started the coroutine and has won then start coroutine
        if (hasWon && !startedCoroutine) {
            startedCoroutine = true;
            StartCoroutine(WinDelay());
        }
    }
    // function for checking number of enemies left
    static public void CheckWin()
    {
        if (EnemyAI.alleys.Count == 0) {
            hasWon = true;
        }
    }
    IEnumerator WinDelay() {
        // waits 10 seconds before switching scenes
        yield return new WaitForSeconds(10);
        // check if haswon is still true, noticed a bug if the play left to menu after all
        // enemies had been killed and would switch scenes straight away
        Debug.Log(hasWon);
        if (hasWon) ChangeScene.NextScene("GameOverWin");
        
    }
}
