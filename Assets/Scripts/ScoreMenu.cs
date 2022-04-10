using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreMenu : MonoBehaviour
{
    // function for score menu button to go to start menu
    public void Play() {
        ChangeScene.NextScene("StartMenu");
    }
}
