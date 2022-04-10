using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyCounter : MonoBehaviour
{
    void Start()
    {
        SetEnemyCount();
    }
    // Displays enemies left
    public void SetEnemyCount() {
        gameObject.GetComponent<Text>().text = "Enemies Left: " + EnemyAI.alleys.Count;
    }
}
