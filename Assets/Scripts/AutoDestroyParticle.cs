using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyParticle : MonoBehaviour
{
    private ParticleSystem effect;
    // small script for destroying particle effects once they're done playing
    void Start()
    {
        effect = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if(effect) {
            if(!effect.IsAlive()) {
                Destroy(gameObject);
            }
        }
    }
}
