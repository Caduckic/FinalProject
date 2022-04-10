using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public bool isShield;
    public bool isHealth;
    public GameObject effect;
    public GameObject pointLight;
    void OnTriggerEnter(Collider other) {
        // checks what kind of powerup it is, and activates the powerup function on the playership
        if (isShield) {
            other.gameObject.GetComponentInParent<ShipController>().PowerupOn("shield");
        }
        if (isHealth) {
            other.gameObject.GetComponentInParent<ShipController>().PowerupOn("health");
        }
        // plays sound effect then destroys itself
        gameObject.GetComponent<AudioSource>().Play();
        Destroy(gameObject.GetComponent<SphereCollider>());
        Destroy(effect);
        Destroy(pointLight);
    }
}
