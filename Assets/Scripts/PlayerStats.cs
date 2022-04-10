using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public GameObject enemies, asteroids, levelsize, time;
    void Start() {
        gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f,1f,1f);
    }
    // part of the playerStats prefab, just sets all its components text fields to inputted ScoreData params
    public void InputStats(ScoreData stats) {
        enemies.GetComponent<TextMeshProUGUI>().text = stats.enemies.ToString();
        asteroids.GetComponent<TextMeshProUGUI>().text = stats.asteroids.ToString();
        levelsize.GetComponent<TextMeshProUGUI>().text = stats.levelSize.ToString("F2");
        time.GetComponent<TextMeshProUGUI>().text = stats.time;
    }
}
