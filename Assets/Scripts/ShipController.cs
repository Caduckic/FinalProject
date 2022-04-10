using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ShipController : MonoBehaviour
{
    public float forwardSpeed = 25f, horStrafeSpeed = 7.5f, vertStrafeSpeed = 5f;
    private float activeForwardSpeed, activeHorStrafeSpeed, activeVertStrafeSpeed;
    private float forwardAcceleration = 2.5f, horStrafeAcceleration = 2f, vertStrafeAcceleration = 2f;
    private float rollInput;
    public float rollSpeed = 120f, rollAcceleration = 2.5f;
    private float turnSpeed = 15f;
    public Camera cam;
    private float baseFOV = 60f;
    private float maxFOV = 90f;
    public GameObject shipObject;
    public Transform shootPoint;
    public GameObject rocket;
    public GameObject rocketParent;
    static public List<GameObject> targetsInView = new List<GameObject>();
    static public List<GameObject> targetsLockable = new List<GameObject>();
    bool locked;
    public bool targetFound = false;
    bool gatheringTargets;
    public GameObject crossHair;
    public GameObject findRangeUI;
    public GameObject lockRangeUI;
    static public GameObject target = null;
    public int MAXLockDistance = 80;
    public int MINlockDistance = 10;
    bool coolDown = false;
    bool inLevel;
    private bool canShoot = true, fireButtonDown = false;
    private int rocketsShot = 0;
    public GameObject boosterMain, boosterRight, boosterLeft;
    private ParticleSystem.EmissionModule boosterRate;
    private List<GameObject> enemies = new List<GameObject>();
    private float rotX, rotY, rotZ, rotationTime = 0.6f, rotMaxAngle = 10f;
    private Vector3 aimDir = new Vector3(-180, 0, 180);
    public GameObject forceShield, healthIncrease;
    private bool hasShield = false, restartShield = false;
    public int currentHealth;
    public int maxHealth = 5;
    private bool wasHit = false;
    private bool hasDied = false;
    public GameObject particleEffect, particleEffectParent;
    private AudioSource _booster, _missleLaunched, _incomingMissile;
    static public bool notSeen = true, seen, rocketClose, enemiesUnaware, winTheme = false;
    
    // uses awake in order for the static music bools to actually reset when reloading playscene
    void Awake()
    {
        notSeen = true;
        seen = false;
        rocketClose = false;
        enemiesUnaware = false;
        winTheme = false;
        if (OptionsMenu.selectedColor != null) {
            shipObject.GetComponent<Renderer>().material.SetTexture("_MainTex", OptionsMenu.selectedColor);
        }
        _booster = boosterMain.GetComponent<AudioSource>();
        _missleLaunched = shootPoint.gameObject.GetComponent<AudioSource>();
        _incomingMissile = shipObject.GetComponent<AudioSource>();
        HealthBar.SetMaxHealth(maxHealth);
        currentHealth = maxHealth;
        // gets enemies from enemy alleys list
        enemies = EnemyAI.alleys;
        // gets inLevel from outofbounds script
        inLevel = OutOfBounds.inLevel;
        // sets all UI active to false and cam to shipController camera, makes code less long 
        crossHair.SetActive(false);
        lockRangeUI.SetActive(false);
        findRangeUI.SetActive(false);
        // sets forceShield off and prevents health increase from playing
        forceShield.SetActive(false);
        healthIncrease.GetComponent<ParticleSystem>().Stop(true);
        //cam = GameObject.Find("Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        MusicState();
        if (!hasDied) {
            // handles movement inputs and effects transform positon in result
            MovementInput();
            // if player presses fire button, rocketsShot is less than max it can shoot at once (2) and canShoot, spawns rocket
            if (Input.GetAxis("Fire1") > 0 && rocketsShot < 2 && canShoot) {
                // in order to treat GetAxis as if its like GetButtonDown I have a bool for if its been pressed or not
                if (!fireButtonDown) {
                    _missleLaunched.Play();
                    // if a target has been found, pass the target into createChildPrefab function
                    if (!targetFound) CreateChild.CreateChildPrefab(rocket, rocketParent, shootPoint, shootPoint.rotation, null, true);
                    else CreateChild.CreateChildPrefab(rocket, rocketParent, shootPoint, shootPoint.rotation, target, true);
                    fireButtonDown = true;
                    // sets canShoot to false and starts a cool down
                    canShoot = false;
                    StartCoroutine(BetweenShotCoolDown());
                    // addes to rockets shot value by 1 and starts reloading
                    rocketsShot++;
                    StartCoroutine(ReloadShot());
                }
            }
            // ensures fire1 input isn't constantly true while holding the button
            else if (Input.GetAxis("Fire1") <= 0) {
                fireButtonDown = false;
            }
            // if there are targets in view and not cooling down, check if any are in lockable range and set gatheringTargets to true
            if (targetsInView.Count > 0 && !gatheringTargets && !locked && !coolDown) {
                gatheringTargets = true;
                CheckOnScreenTargets();
            }

            // if there is a target draw the crossHair over its position
            if (targetFound && target != null) {
                crossHair.SetActive(true);
                lockRangeUI.SetActive(true);
                crossHair.transform.position = cam.WorldToScreenPoint(target.transform.position);
            }
            // if crossHair is active and there is a target, if the crosshair moves outside of given screen position
            // set targets layer to default (which is 0, this is done for how the rockets aim at targets using raycasts)
            // and clears targets
            if (crossHair.activeSelf && target != null) {
                if (crossHair.transform.position.x < Screen.width / 5 || crossHair.transform.position.x > (Screen.width / 5) * 4
                    || crossHair.transform.position.y < Screen.height / 5 || crossHair.transform.position.y > (Screen.height / 5) * 4
                        || Vector3.Distance(target.transform.position, transform.position) > MAXLockDistance + (MAXLockDistance / 3)) {
                        target.layer = 0;
                        ClearTargets();
                }
            }
            // if target suddenly is null while target found (such as rocket destroying it), clear targets
            if (target == null && !gatheringTargets && targetFound) {
                ClearTargets();
            }
            // does sets emission rate/intensity of booster particles based on if going forwards or not
            if (Input.GetAxisRaw("Excelerate") < 0) {
                boosterRate = boosterMain.GetComponent<ParticleSystem>().emission;
                boosterRate.rateOverTime = 10;
            }
            else {
                boosterRate = boosterMain.GetComponent<ParticleSystem>().emission;
                boosterRate.rateOverTime = Mathf.Lerp(boosterRate.rateOverTimeMultiplier, 0, 3f * Time.deltaTime);
            }
            // just sets left and right emissions to the same as the main
            var rateLeft = boosterLeft.GetComponent<ParticleSystem>().emission;
            var rateRight = boosterRight.GetComponent<ParticleSystem>().emission;
            rateLeft.rateOverTime = boosterRate.rateOverTime;
            rateRight.rateOverTime = boosterRate.rateOverTime;

            _booster.pitch = Mathf.Lerp(_booster.pitch, Input.GetAxisRaw("Excelerate"), Time.deltaTime / 0.5f);
        }
    }
    void MovementInput() {
        // takes input and uses mathf.lerp to slowly transition movement values
        rollInput = Mathf.Lerp(rollInput, Input.GetAxisRaw("Roll"), rollAcceleration * Time.deltaTime);

        transform.Rotate(activeVertStrafeSpeed * turnSpeed * Time.deltaTime, activeHorStrafeSpeed * turnSpeed * Time.deltaTime, rollInput * rollSpeed * Time.deltaTime, Space.Self);

        activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, Input.GetAxisRaw("Excelerate") * forwardSpeed, forwardAcceleration * Time.deltaTime);
        
        activeHorStrafeSpeed = Mathf.Lerp(activeHorStrafeSpeed, Input.GetAxisRaw("Horizontal") * horStrafeSpeed, horStrafeAcceleration * Time.deltaTime);
        activeVertStrafeSpeed = Mathf.Lerp(activeVertStrafeSpeed, Input.GetAxisRaw("Vertical") * vertStrafeSpeed, vertStrafeAcceleration * Time.deltaTime);

        // takes movement vaules and applies them to tranform position using appropriote tranfrom direction
        // Thanks gamesplusjames for helping me start this, when I got the idea to do a space game
        // I searched for how to make a basic spaceship controller to start, his video really got me started!
        // Check out this video https://www.youtube.com/watch?v=J6QR4KzNeJU to compare my controller to the one he shows in the video to see the differences
        transform.position += transform.forward * activeForwardSpeed * Time.deltaTime;
        transform.position += transform.right * activeHorStrafeSpeed * Time.deltaTime;
        transform.position += transform.up * activeVertStrafeSpeed * Time.deltaTime;

        // increases fov while moving forwards, gives the player a better sense of speed
        cam.fieldOfView = Mathf.Lerp(baseFOV, maxFOV, -activeForwardSpeed / 5);

        // handles rotating the ship a little in the direction its turning to, a request from play testers
        // I really like how it's turned out!
        // tried a lot of different methods but the answer from Scribe here helped the most
        // https://answers.unity.com/questions/640938/slowing-down-rotation-speed.html
        if (Input.GetAxisRaw("Horizontal") < 0) {
            // uses input value (goes from -1 to 1) as for controllers it works better
            // to only move the rotation according to how much the player has moved the joystick
            rotX = -rotMaxAngle * -Input.GetAxisRaw("Horizontal");
        }
        else if (Input.GetAxisRaw("Horizontal") > 0) {
            rotX = rotMaxAngle * Input.GetAxisRaw("Horizontal");
        }
        else rotX = 0;
        if (Input.GetAxisRaw("Vertical") < 0) {
            rotY = -rotMaxAngle * -Input.GetAxisRaw("Vertical");
        }
        else if (Input.GetAxisRaw("Vertical") > 0) {
            rotY = rotMaxAngle * Input.GetAxisRaw("Vertical");
        }
        else rotY = 0;
        if (Input.GetAxisRaw("Roll") < 0) {
            rotZ = -rotMaxAngle * Input.GetAxisRaw("Roll");
        }
        else if (Input.GetAxisRaw("Roll") > 0) {
            rotZ = -rotMaxAngle * Input.GetAxisRaw("Roll");
        }
        else rotZ = 0;
        // after getting current rotXYZ vaules, vector3.lerp to a vector with said vaules, at time/rotationTime
        // then apply aimDir to ships local eular angles
        aimDir = Vector3.Lerp(aimDir, new Vector3 (rotY - 180, rotX, 180 + rotZ), Time.deltaTime/rotationTime);
        shipObject.transform.localEulerAngles = aimDir;
    }
    void CheckOnScreenTargets() {
        bool foundValidTarget = false;
        // iterates over rendered targets/all enemies in scene (targetsInView)
        // to check if any are within lock on distance and lock on screenspace
        for (int i = 0; i < targetsInView.Count; i++) {
            // console was complaining about seeing null objects when I reset the scene so I'm accounting for it here
            if (targetsInView[i] != null) {
                Vector3 targetPos = cam.WorldToViewportPoint(targetsInView[i].transform.position);
                if (targetPos.z > MINlockDistance && targetPos.z < MAXLockDistance && targetPos.x > 0.33f && targetPos.x < 0.66f && targetPos.y > 0.33f && targetPos.y < 0.66f) {
                    // if 1 target has been found set foundValidTarget to true and break out of loop
                    // we don't want to keep looping if we already now know we could lock on
                    foundValidTarget = true;
                    break;
                }
            }
            
        }
        // if a target was found in range set finding targets UI (findRangeUI) on and Gather all targets
        if (foundValidTarget == true) {
            findRangeUI.SetActive(true);
            // this is a coroutine, you'll see why futher down
            StartCoroutine(GatherTargets());
        }
        // if none were found then set gathering Targets to false so it can try again
        else {
            gatheringTargets = false;
        }
    }
    // clears all target data and disables targeting UI
    // also sets coolDown true to stop player instantly locking on again
    void ClearTargets() {
        target = null;
        targetsLockable.Clear();
        crossHair.SetActive(false);
        lockRangeUI.SetActive(false);
        targetFound = false;
        locked = false;
        coolDown = true;
        StartCoroutine(LockOnCoolDown());
    }

    // waits for 1 secound before added gameobjects to list of lockable targets
    // then if it found any, set target to closest of them all/enemy

    // the reason why I wait before doing this is if I don't as soon as 1 object is found
    // its the only object that'll be added, making targeting very annoying for the player
    // it also makes it so it finds a target instantly which I don't want
    // I want it to act as if the ship is doing its own calulations to find the target before deciding on one
    IEnumerator GatherTargets() {
        yield return new WaitForSeconds(1f);
        // this if statement was here before I had enemies always be inside targetsInView,
        // though it can stay considering a level perhaps without enemies
        if (targetsInView.Count > 0) {
            for (int i = 0; i < targetsInView.Count; i++) {
                if (targetsInView[i] != null) {
                    Vector3 targetPos = cam.WorldToViewportPoint(targetsInView[i].transform.position);
                    if (targetPos.z > 10 && targetPos.z < 70 && targetPos.x > 0.33f && targetPos.x < 0.66f && targetPos.y > 0.33f && targetPos.y < 0.66f && !targetsLockable.Contains(targetsInView[i])) {
                        targetsLockable.Add(targetsInView[i]);
                        
                    }
                }
                
            }
            if (targetsLockable.Count > 0) {
                target = FindClosestTarget();
            }
        }
        findRangeUI.SetActive(false);
        gatheringTargets = false;
    }
    // Returns the target gameobject by comparing all lockable targets distance and finding the smallest.
    // alteratively if it finds a enemy in that list, it will will set that to the closest target
    // and break out of the loop regardless of if its futher away than other objects in the list.
    // this is for the players to not get annoyed when trying to target an enemy but keep getting locked
    // on to other objects just because they're closer
    GameObject FindClosestTarget() {
        float closestDist = Mathf.Infinity;
        GameObject closestTarget = null;
        Vector3 currentPosition = transform.position;
        for (int i = 0; i < targetsLockable.Count; i++) {
            Vector3 distToTarget = targetsLockable[i].transform.position - currentPosition;
            float distToTargetSqr = distToTarget.sqrMagnitude;
            if (distToTargetSqr < closestDist) {
                closestDist = distToTargetSqr;
                closestTarget = targetsLockable[i];
            }
            if (targetsLockable[i].tag == "Enemy") {
                closestTarget = targetsLockable[i];
                break;
            }
        }
        locked = true;
        targetFound = true;
        gatheringTargets = false;
        closestTarget.layer = 8;
        return closestTarget;
    }
    // function for applying powerup effects
    public void PowerupOn(string powerupType) {
        if (powerupType == "shield") {
            // these if statements are for if the player while having a shield already
            // gets another shield powerup. if so the timer will restart so they can keep their shield
            if (!hasShield) {
                forceShield.SetActive(true);
                hasShield = true;
                StartCoroutine(ShieldTimer());
            }
            else {
                restartShield = true;
                StartCoroutine(ShieldTimer());
            }
        }
        if (powerupType == "health") {
            // plays health particle effect
            healthIncrease.GetComponent<ParticleSystem>().Play(true);
            // if the player has health value of only 1 then once the powerup is collected, set its _lowHealth to 0 (this is the red glowing)
            if (currentHealth == 1) shipObject.GetComponent<MeshRenderer>().material.SetInt("_lowHealth", 0);
            // sets health to max health and changes the healthbar to current health
            currentHealth = maxHealth;
            HealthBar.SetHealth(currentHealth);
        }
    }
    public void TakeDamage() {
        // if not already hit and doesn't have a shield then washit = true and health goes down
        if (!wasHit && !hasShield) {
            wasHit = true;
            currentHealth--;
            // sets healthbar to current health
            HealthBar.SetHealth(currentHealth);
            // sets ship shader values in order to have the ship glow on and off red
            shipObject.GetComponent<MeshRenderer>().material.SetInt("_lowHealth", 1);
            shipObject.GetComponent<MeshRenderer>().material.SetFloat("_speed", 15f);
            // starts a cool down coroutine
            StartCoroutine(HitCoolDown());
        }
        // if health is 0 then GameOver
        if (currentHealth == 0) {
            GameOver();
        }
    }
    void GameOver() {
        // creates dust particle effect
        CreateChild.CreateChildPrefab(particleEffect, particleEffectParent, shipObject.transform, Random.rotation);
        // this is so there is still an audio listener in the scene, I put it on the ship object
        var Audio = Instantiate(new GameObject(), shipObject.transform.position, shipObject.transform.rotation);
        // creates a copy gameObject, this is so the audio doesn't cut off, it has the audio listener
        Audio.AddComponent<AudioListener>();
        // destroys the ship
        Destroy(shipObject);
        // sets off all UI elements
        crossHair.SetActive(false);
        lockRangeUI.SetActive(false);
        findRangeUI.SetActive(false);
        // sets has died to true and starts GameOverTimer
        hasDied = true;
        StartCoroutine(GameOverTimer());
    }
    // sets all the bools on and off within given params, used later in the MusicController
    void MusicState() {
        if (!seen) {
            // if any enemies see the player then seen is true
            for (int i = 0; i < enemies.Count; i++) {
                if (enemies[i].GetComponent<EnemyAI>().foundPlayer) {
                    notSeen = false;
                    rocketClose = false;
                    enemiesUnaware = false;
                    seen = true;
                    break;
                }
            }
        }
        if (!notSeen && !seen && !enemiesUnaware) {
            seen = false;
            rocketClose = false;
            enemiesUnaware = false;
            notSeen = true;
        }
        if (!rocketClose && RocketBehaviour.closeToPlayer) {
            notSeen = false;
            seen = false;
            enemiesUnaware = false;
            rocketClose = true;
        }
        if (seen && !enemiesUnaware || rocketClose && !enemiesUnaware) {
            var count = 0;
            for (int i = 0; i < enemies.Count; i++) {
                if (!enemies[i].GetComponent<EnemyAI>().foundPlayer) {
                    count++;
                }
                if (count == enemies.Count) {
                    notSeen = false;
                    rocketClose = false;
                    seen = false;
                    enemiesUnaware = true;
                }
            }
        }
        if (enemies.Count == 0) {
            notSeen = false;
            seen = false;
            enemiesUnaware = false;
            rocketClose = false;
            winTheme = true;
        }
        // used to make sure only one is active at a time
        //Debug.Log("1=" + seen + " 2=" + notSeen + " 3=" + rocketClose + " 4=" + enemiesUnaware + " 5=" + winTheme);
    }
    // sets cool down between player shots, just to add a little more timing between shots
    IEnumerator BetweenShotCoolDown() {
        yield return new WaitForSeconds(0.2f);
        canShoot = true;
    }
    // sets rockets shot back minus 1 after 1 secound so player can shoot again
    IEnumerator ReloadShot() {
        yield return new WaitForSeconds(1f);
        rocketsShot--;
    }
    // simply waits for 2 secounds and then sets coolDown back to false as to finish cool down
    IEnumerator LockOnCoolDown() {
        yield return new WaitForSeconds(2f);
        coolDown = false;
    }
    IEnumerator ShieldTimer() {
        yield return new WaitForSeconds(20f);
        if (!restartShield) {
            forceShield.SetActive(false);
            hasShield = false;
        }
        else {
            restartShield = false;
        }
    }
    IEnumerator HitCoolDown() {
        yield return new WaitForSeconds(2.5f);
        wasHit = false;
        if (shipObject != null) {
            if (currentHealth > 1) shipObject.GetComponent<MeshRenderer>().material.SetInt("_lowHealth", 0);
            else shipObject.GetComponent<MeshRenderer>().material.SetFloat("_speed", 5f);
        }
    }
    IEnumerator GameOverTimer() {
        yield return new WaitForSeconds (5f);
        ChangeScene.NextScene("StartMenu");
    }
}
