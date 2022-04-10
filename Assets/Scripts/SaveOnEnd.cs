using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveOnEnd : MonoBehaviour
{
    public GameObject playerScores;
    void Start()
    {
        // opens the scores file and gets back the list of ScoreData
        List<ScoreData> scores = ScoreManager.LoadScores();
        // if it exists save the new score while passing in the ScoreData list
        if (scores != null) ScoreManager.SaveData(scores);
        // else save new score without inputing it
        // (probably useless as this function checks null already)
        else {
            ScoreManager.SaveData();
        }
        // creates score ui Elements and sets their vaules
        playerScores.GetComponent<DisplayScores>().Display(scores);
    }
}
