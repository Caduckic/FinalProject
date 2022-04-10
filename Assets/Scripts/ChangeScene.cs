using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    // changes the scene and importantly resets all the lists
    static public void NextScene(string scene) {
        EnemyAI.alleys.Clear();
        ShipController.targetsInView.Clear();
        ShipController.targetsLockable.Clear();
        // needs to set hasWon to false so that the coroutine doesn't get run
        CheckForWin.hasWon = false;
        SceneManager.LoadScene(scene);
    }
}
