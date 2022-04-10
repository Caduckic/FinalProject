using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float forwardSpeed = 23f, rotationSpeed = 6, closingInSpeed = 1;
    public Transform shootPoint;
    public GameObject rocket;
    public GameObject rocketParent;
    public GameObject player;
    public GameObject particleEffect, particleParent;
    public float shootingDistance = 80f;
    public float flyAwayDistance = 30f;
    public float searchRadius = 150f;
    private Quaternion randomDirection;
    private bool hasShot = false, flyAway = false, pickingDirection = false, closingIn = false;
    [HideInInspector] public bool foundPlayer = false;
    private float viewAngle = 15f;
    public float alleyDistMIN = 2f;
    private bool alleyTooClose = false;
    static public List<GameObject> alleys = new List<GameObject>();
    private bool inLevel = true;
    public GameObject levelCenter;
    private bool playerFollowing = false;
    public Camera playerCam; 
    public GameObject arrowPointer;
    public GameObject canvas;
    private GameObject arrow;
    private RectTransform canvasRect;
    private bool failedChasePlayer = false;
    private Vector3 oldPlayerPos;
    private AudioSource _missileLaunched;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PlayerShip");
        particleParent = GameObject.Find("ParticleParent");
        rocketParent = GameObject.Find("RocketParent");
        canvas = GameObject.Find("Canvas");
        levelCenter = GameObject.Find("LevelSize");
        _missileLaunched = shootPoint.gameObject.GetComponent<AudioSource>();
        playerCam = GameObject.Find("Camera").GetComponent<Camera>();
        canvasRect = canvas.GetComponent<RectTransform>();
        arrow = Instantiate(arrowPointer);
        arrow.transform.SetParent(canvas.transform);
        arrow.SetActive(false);
        // adds self to enemy alleys list, sets enemycount, choses random direction and adds self to players target in view list
        alleys.Add(gameObject);
        GameObject.Find("EnemiesLeft").GetComponent<EnemyCounter>().SetEnemyCount();
        randomDirection = Random.rotation;
        ShipController.targetsInView.Add(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // based on set values such as distance, changes bools that effect movement behaviour
        // checks if player is null for when the player is destroyed
        if (player != null) AttackBehaviour();
        // based on bool values set in attack behaviour sets movement mode
        Movement();
        // checks if players camera can see this enemy and moves arrow towards itself
        PointAtSelf();
    }
    void Movement() {
        // checks distance from level center based on maxLevelSize (a radius) and switches in level to false while outside it
        if (Vector3.Distance(levelCenter.transform.position, transform.position) > LevelGenerator.maxLevelSize) inLevel = false;
        // checks all alleys in alleys list that aren't itself and compares distance.
        // if alley is too close it will rotation away from them, this stops them clipping into each other
        foreach (GameObject alley in alleys) {
            if (alley != gameObject) {
                if (Vector3.Distance(alley.transform.position, transform.position) < alleyDistMIN) {
                    alleyTooClose = true;
                    LookTowards(transform, alley.transform, rotationSpeed);
                }
                else alleyTooClose = false;
            }
        }
        //Debug.Log(alleyTooClose);
        // Constantly flys in a forward direction
        transform.position += transform.forward * forwardSpeed * Time.deltaTime;
        if (player != null) {
            // if enemy has found the player and isn't flying away the enemy will move towards them
            if (foundPlayer && !flyAway && inLevel && !alleyTooClose) {
                // if enemy not at closing in distance it'll use regular rotationspeed, else it'll use closing in rotation speed
                if (!closingIn) {
                    LookTowards(player.transform, transform, rotationSpeed);
                }
                else {
                    LookTowards(player.transform, transform, closingInSpeed);
                }
            }
            // if flyAway then aim away from player
            else if (foundPlayer && flyAway && inLevel && !alleyTooClose) {
                LookTowards(transform, player.transform, rotationSpeed);
            }
        }
        else foundPlayer = false;
        // if player hasn't been found, choose a random rotation and aim there
        if (!foundPlayer && inLevel && !alleyTooClose) {
            if (!pickingDirection) {
                pickingDirection = true;
                StartCoroutine(PickRandomDirection());
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, randomDirection, Time.deltaTime / rotationSpeed);
        }
        // if not inLevel (this is used over all movement to stop ships going out of bounds just because the player is)
        // aim towards the center of the level
        else if (!inLevel) {
            LookTowards(levelCenter.transform, transform, closingInSpeed);
            StartCoroutine(ReturnToLevel());
        }
    }
    // function for moving enemies rotation, aims at point1 from point2's position.
    void LookTowards(Transform point1, Transform point2, float rotationSpeed) {
        Vector3 lookDirection = point1.position - point2.position;
        lookDirection = lookDirection.normalized;
        if (lookDirection != Vector3.zero) {
            var rotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime / rotationSpeed);
        }
    }
    void AttackBehaviour() {
        // if player has come within enemy search radius enemy has found player
        if (Vector3.Distance(player.transform.position, transform.position) < searchRadius && !flyAway && !foundPlayer) {
            //Debug.Log(gameObject.name + "has found player");
            foundPlayer = true;
        }
        // if player has left double enemie search radius, enemy loses player
        if (Vector3.Distance(player.transform.position, transform.position) > searchRadius * 2 && foundPlayer) {
            //Debug.Log(gameObject.name + "has lost player");
            closingIn = false;
            flyAway = false;
            foundPlayer = false;
        }
        // if enemy within 100 units from player, close in
        if (Vector3.Distance(player.transform.position, transform.position) < 100 && !closingIn && !failedChasePlayer) {
            //Debug.Log(gameObject.name + "is closing in");
            //Debug.Log("closing in at" + Vector3.Distance(player.transform.position, transform.position));
            closingIn = true;
            oldPlayerPos = player.transform.position;
            StartCoroutine(ChasePlayerTimer());
        }
        // if enemy within a 80 units, hasn't already shot and player within its view angle (so they can't shoot player from behind itself)
        // shoot rocket at player
        if (Vector3.Distance(player.transform.position, transform.position) < shootingDistance &&
                Vector3.Distance(player.transform.position, transform.position) > flyAwayDistance + 10f && closingIn && !hasShot 
                && Vector3.Angle(transform.forward, player.transform.position - transform.position) < viewAngle) {
                    hasShot = true;
                    StartCoroutine(ShootCoolDown());
                    //Debug.Log(Vector3.Distance(player.transform.position, transform.position));
                    _missileLaunched.Play();
                    CreateChild.CreateChildPrefab(rocket, rocketParent, shootPoint, shootPoint.rotation, player);
        }
        // if enemy has closed to to a certain distance, fly away
        if (Vector3.Distance(player.transform.position, transform.position) < flyAwayDistance && closingIn) {
            //Debug.Log("making distance at Vector3.Distance(player.transform.position, transform.position)");
            flyAway = true;
            closingIn = false;
            StartCoroutine(TurnBack());
        }
        // if the player is following enemy, don't turn back until they stop/fail to reach enemy before it reaches 60 units of distance
        if (Vector3.Distance(player.transform.position, transform.position) > 60 && playerFollowing) {
            playerFollowing = false;
            flyAway = false;
            closingIn = true;
            StartCoroutine(QuickTurn());
        }
    }
    // creates explosion particle effect, removes self from targetsInView and alley Lists, changes Enemy Counter and destroys self
    public void DestroyEnemy() {
        CreateChild.CreateChildPrefab(particleEffect, particleParent, transform, Random.rotation);
        ShipController.targetsInView.Remove(gameObject);
        alleys.Remove(gameObject);
        GameObject.Find("EnemiesLeft").GetComponent<EnemyCounter>().SetEnemyCount();
        CheckForWin.CheckWin();
        Destroy(arrow);
        Destroy(gameObject);
    }
    // this kind of function was a bit too hard for me to figure out so thank you digijin!
    // check out this video I found of his that helped me on this project
    // https://www.youtube.com/watch?v=gAQpR1GN0Os
    void PointAtSelf() {
        Vector3 enemyPos = playerCam.WorldToScreenPoint(transform.position);
        if (enemyPos.z > 0 && enemyPos.x > 0 && enemyPos.x < Screen.width 
            && enemyPos.y > 0 && enemyPos.y < Screen.height) {
                arrow.SetActive(false);
            }
        else {
            // flips values, otherwise arrows direct you to the wrong spot
            if (enemyPos.z < 0) enemyPos *= -1;
            // makes 00 the center of the screen
            Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0)/2;
            enemyPos -= screenCenter;
            // finds angle from center of screen
            float angle = Mathf.Atan2(enemyPos.y, enemyPos.x);
            angle -= 90 * Mathf.Deg2Rad;

            float cos = Mathf.Cos(angle);
            float sin = -Mathf.Sin(angle);

            enemyPos = screenCenter + new Vector3(sin*150, cos*150, 0);

            // y =mx+b format is something I do not understand quite honestly
            float m = cos / sin;

            Vector3 screenBounds = screenCenter * 0.9f;

            //checks top and bottom of screen first
            if (cos > 0) enemyPos = new Vector3(screenBounds.y/m, screenBounds.y, 0);
            else enemyPos = new Vector3(-screenBounds.y/m, -screenBounds.y, 0);
            // if out of bounds (must be on either left or right)
            if (enemyPos.x > screenBounds.x) enemyPos = new Vector3(screenBounds.x, screenBounds.x*m, 0);
            else if (enemyPos.x < - screenBounds.x) enemyPos = new Vector3(-screenBounds.x, - screenBounds.x*m, 0);

            // defaults positions back to 00 being bottom left
            enemyPos += screenCenter;

            //sets arrow on and sets calulated enemyPos to its position
            arrow.SetActive(true);
            // originally localPositon but this made the arrows go off the screen based on screen scale/size
            arrow.transform.position = enemyPos;
            arrow.transform.localRotation = Quaternion.Euler(0, 0, angle*Mathf.Rad2Deg);
        }
    }
    // waits every 4 secounds and picks a random direction if player hasn't been found
    IEnumerator PickRandomDirection() {
        yield return new WaitForSeconds(4f);
        if (!foundPlayer) {
            randomDirection = Random.rotation;
            pickingDirection = false;
        }
        else pickingDirection = false;
    }
    // these chasing player fuctions I put here to nerf the ai a bit, without these
    // the ai would keep its faster rotation speed and chase the player forever with it
    // this made it basically impossible to turn around and not get hit with every rocket the ai had shot
    IEnumerator ChasePlayerTimer() {
        yield return new WaitForSeconds(2f);
        if (player != null) {
            // I check how far the player has moved here because I noticed the ai would just orbit the player
            // and never shoot again because the player would never line up with their direction
            if (closingIn && Vector3.Distance(oldPlayerPos, player.transform.position) > 25) {
                closingIn = false;
                failedChasePlayer = true;
                StartCoroutine(ChasePlayerFailed());
            }
        }
    }
    // just resets failed to chase player back after 5 secounds, giving the player time to escape
    IEnumerator ChasePlayerFailed() {
        yield return new WaitForSeconds(5f);
    failedChasePlayer = false;
    }
    // if player is futher than 60 units away (most likely meaning they didn't follow enemy)
    // turn back towards them, else set playerFollowing to true
    IEnumerator TurnBack() {
        yield return new WaitForSeconds(7f);
        //Debug.Log("turning back at " + Vector3.Distance(player.transform.position, transform.position));
        if (player != null) {
            if (Vector3.Distance(player.transform.position, transform.position) > 60) {
                flyAway = false;
                closingIn = true;
                StartCoroutine(QuickTurn());
            }
            else playerFollowing = true;
        }
    }
    // lets closing in speed stay on for 2 secounds before switching back to regular rotation speed
    IEnumerator QuickTurn() {
        yield return new WaitForSeconds(2f);
        closingIn = false;
    }
    // doesn't set inLevel back to true for 2.5 secounds, this gives it time to properly face the center of the level
    IEnumerator ReturnToLevel() {
        yield return new WaitForSeconds(2.5f);
        inLevel = true;
    }
    // doesn't set hasShot back to false for 3.5 secounds, used so enemies can't just shoot nonstop while having the needed other varibles for shooting
    IEnumerator ShootCoolDown() {
        yield return new WaitForSeconds(3.5f);
        hasShot = false;
    }
}
