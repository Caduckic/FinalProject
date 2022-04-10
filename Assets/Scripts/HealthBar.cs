using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    // followed brackeys video on health bars
    // https://www.youtube.com/watch?v=BLfNP4Sc_iA
    static public Slider slider;
    void Start() {
        slider = gameObject.GetComponent<Slider>();
    }
    static public void SetMaxHealth(int health) {
        if (slider != null) {
            slider.maxValue = health;
            slider.value = health;
        }
    }
    static public void SetHealth(int health) {
        slider.value = health;
    }
}
