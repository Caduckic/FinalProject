using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    static public bool inLevel;
    public GameObject warningText;
    // uses a large Sphere collider changed in the level generator script to tell if the player is within it
    void OnTriggerEnter(Collider other) {
        // sets the warning text back off
        if (other.gameObject.transform.parent.parent.name == "PlayerShip") {
            inLevel = true;
            if (warningText.activeSelf) {
                warningText.SetActive(false);
            }
        }
    }
    // if the player exits the Sphere, a warning message pops up with a countdown
    void OnTriggerExit(Collider other) {
        if (other.gameObject.transform.parent.parent.name == "PlayerShip") {
            inLevel = false;
            if (!warningText.activeSelf) {
                warningText.SetActive(true);
            }
        }
    }
}
