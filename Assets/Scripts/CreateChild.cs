using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateChild : MonoBehaviour
{
    // very similar to the function used in the lectures, with some added features
    static public void CreateChildPrefab(GameObject prefab, GameObject parent, Transform position, Quaternion rotation, GameObject target = null, bool playerRocket = false) {
        var myPrefab = Instantiate(prefab, position.position, rotation);
        myPrefab.transform.parent = parent.transform;
        // if the prefab is a rocket it can have a target, if it does then pass it into its script
        if (target != null) {
            myPrefab.GetComponent<RocketBehaviour>().target = target.transform;
        }
        // rockets can also be marked playerRocket, this is for when the players shield is up the player can still shoot
        if (playerRocket) {
            myPrefab.GetComponent<RocketBehaviour>().playerRocket = playerRocket;
        }
    }
}
