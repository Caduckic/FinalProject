using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject asteroidParent, enemyParent;
    public GameObject asteroid1, asteroid2, asteroid3, asteroid4, asteroid5, enemyShip;
    public GameObject levelSphere;
    public float generateSize;
    public int asteroidCount, enemyCount;
    static public float maxLevelSize;
    // Start is called before the first frame update
    void Start()
    {
        // takes values inputted from options menu and applies them to generation values
        generateSize = OptionsMenu.levelsize;
        asteroidCount = OptionsMenu.asteroids;
        enemyCount = OptionsMenu.enemies;
        // uses list of asteroid types and adds all models
        List<GameObject> asteroids = new List<GameObject>();
        asteroids.Add(asteroid1);
        asteroids.Add(asteroid2);
        asteroids.Add(asteroid3);
        asteroids.Add(asteroid4);
        asteroids.Add(asteroid5);
        var randomPos = new GameObject().transform;
        // max level size a bit larger than spawnable asteroid area (levelsize)
        maxLevelSize = generateSize + 50;
        // sets count to 0, and while count isn't the same as asteroidCount, randomly place them and count up
        int count = 0;
        while (count != asteroidCount) {
            // randomly selects model
            GameObject asteroid = asteroids[Random.Range(0, asteroids.Count - 1)];
            // randomly places asteroid within the level as a Sphere
            randomPos.position = Random.insideUnitSphere * generateSize;
            CreateChild.CreateChildPrefab(asteroid, asteroidParent, randomPos, Random.rotation);
            count++;
        }
        // uses count again but now with enemies
        count = 0;
        while (count != enemyCount) {
            // randomly places enemies this time on the Sphere instead of inside like the asteroids
            randomPos.position = Random.onUnitSphere * generateSize;
            CreateChild.CreateChildPrefab(enemyShip, enemyParent, randomPos, Random.rotation);
            count++;
        }
        // sets levelSphere collider radius to maxLevelSize
        levelSphere.GetComponent<SphereCollider>().radius = maxLevelSize;
    }
}
