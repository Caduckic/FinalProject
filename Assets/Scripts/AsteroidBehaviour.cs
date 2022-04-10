using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidBehaviour : MonoBehaviour
{
    public GameObject particleEffect;
    private GameObject ParticleParent;
    private GameObject PowerupParent;
    public GameObject Health;
    public GameObject Shield;
    Renderer astRenderer;
    private bool isPowerup = false;
    // we have a start here to randomly select if its going to drop a powerup or not
    // marked by it having a glowing shader I made
    void Start()
    {
        astRenderer = GetComponent<Renderer>();
        // using Find here as I couldn't add them before hand
        ParticleParent = GameObject.Find("ParticleParent");
        PowerupParent = GameObject.Find("PowerupParent");
        // randomly sets if asteroid is a powerup and sets it to glow
        if (Random.Range(0, 10) == 1) {
            GetComponent<MeshRenderer>().material.SetFloat("_glow", 0.6f);
            isPowerup = true;
        }
        else GetComponent<MeshRenderer>().material.SetFloat("_glow", 1f);
    }

    // Update is called once per frame
    void Update()
    {
        // if the asteroid is being rendered and isn't already in the list add to the targets in view list
        if (astRenderer.isVisible && !ShipController.targetsInView.Contains(gameObject)) {
            ShipController.targetsInView.Add(gameObject);
            //Debug.Log(ShipController.targetsInView.Count);
        }
        // if its not being rendered then remove from the list
        else if (!astRenderer.isVisible && ShipController.targetsInView.Contains(gameObject)) {
            ShipController.targetsInView.Remove(gameObject);
            //Debug.Log(ShipController.targetsInView.Count);
        }
    }

    void OnTriggerEnter(Collider other) {
        // gives the ateroid a rigid body and sets params, just so the ship doesn't clip through it
        if (gameObject.GetComponent<Rigidbody>() == null) {
            Rigidbody rigid = gameObject.AddComponent<Rigidbody>();
            rigid.mass = 7f;
            rigid.useGravity = false;
            rigid.angularDrag = 0f;
        }
    }

    public void breakAsteroid(GameObject projectile) {
        // creates dust particle effect, removes itself from any lists it may be in
        CreateChild.CreateChildPrefab(particleEffect, ParticleParent, transform, Random.rotation);
        ShipController.targetsInView.Remove(gameObject);
        ShipController.targetsLockable.Remove(gameObject);
        // if its a powerup, randomly select what type and spawn it based on projectile transform
        if (isPowerup) {
            int powerup = Random.Range(0, 2);
            if (powerup == 1) CreateChild.CreateChildPrefab(Health, PowerupParent, transform, projectile.transform.rotation);
            else if (powerup == 0) CreateChild.CreateChildPrefab(Shield, PowerupParent, transform, projectile.transform.rotation);
        }
        // finally destroys the asteroid
        Destroy(gameObject);
    }
}
