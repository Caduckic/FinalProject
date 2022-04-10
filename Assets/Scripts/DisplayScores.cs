using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayScores : MonoBehaviour
{
    public GameObject playerStats;
    public void Display(List<ScoreData> data) {
        // double checks data isn't null
        if (data != null) {
            // for ever ScoreData is the list, create a new playStats Prefab, set self as its parent
            // and set params of playStats to list[i] using custom function
            for (int i = 0; i < data.Count; i++) {
                var stats = Instantiate(playerStats);
                stats.transform.SetParent(gameObject.transform);
                stats.GetComponent<PlayerStats>().InputStats(data[i]);
            }
        }
    }
}
