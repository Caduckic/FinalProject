using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBehaviour : MonoBehaviour
{
    public GameObject explosion;
    private GameObject ParticleParent;
    public float power = 0.1f, radius = 5f;
    private float speed = 35f;
    public Transform target;
    public bool playerRocket;
    private float targetingSpeed = 2.5f;
    private Vector3 direction;
    private Vector3 originRotation;
    bool lostTarget = false;
    private Vector3 oldRotation;
    private bool collided = false;
    [HideInInspector]
    static public bool closeToPlayer = false;
    // Start is called before the first frame update
    void Start()
    {
        originRotation = transform.rotation.eulerAngles;
        ParticleParent = GameObject.Find("ParticleParent");
        StartCoroutine(selfDestruct());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (transform.forward * speed * Time.deltaTime);
        if (target != null && !lostTarget) {
            oldRotation = transform.rotation.eulerAngles;
            //var angle = Vector3.Angle(originRotation, transform.position);
            targetingSpeed += 0.1f;
            direction = target.position - transform.position;
            direction = direction.normalized;
            var rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, targetingSpeed * Time.deltaTime);
            // changes speed based on how much rocket is rotating, helpful for if player is trying to not get it.
            // player will be able to slow the rocket by moving around
            speed += (-Mathf.Abs(oldRotation.x - transform.rotation.eulerAngles.x) / 150) + (-Mathf.Abs(oldRotation.y - transform.rotation.eulerAngles.y) / 150) + (-Mathf.Abs(oldRotation.z - transform.rotation.eulerAngles.z) / 150);
            // once the rocket gets to a certain distance, I use a raycast to check if it can still "see" the target in case the direction was a bit off
            // if I didn't do this it would circle the target very fast as it closed in and I wanted to to just fly pass as if it lost sight
            // this method isn't perfect as it can still circle the target sometimes, but I think it was good enough to not have to spend so much time perfecting it
            if (Vector3.Distance(target.position, transform.position) < 3 && !lostTarget) {
                RaycastHit hit;
                int layerMask = 1 << 8;
                if (!Physics.Raycast(transform.position, transform.forward, out hit, 2, layerMask)) {
                    // resets speed back to oringal
                    speed = 35f;
                    lostTarget = true;
                }
            }
            if (target.gameObject.name == "PlayerShip") {
                closeToPlayer = true;
            }
            else closeToPlayer = false;
        }
    }

    void OnTriggerEnter(Collider other) {
        // due to multiple colliders on one object, these can be called more than once
        // as a quick fix I've just made a bool turn on the first time its called
        // (this might not be needed anymore as I've changed the prefabs to only have a single collider)
        if (!collided) {
            if (other.tag == "Target") {
                other.GetComponent<AsteroidBehaviour>().breakAsteroid(gameObject);
                Explode();
            }
            else if (other.transform.parent.tag == "Enemy") {
                other.transform.parent.parent.parent.gameObject.GetComponent<EnemyAI>().DestroyEnemy();
                Explode();
            }
            else if (other.tag == "Player") {
                other.transform.parent.parent.parent.gameObject.GetComponent<ShipController>().TakeDamage();
                Explode();
            }
            else if (other.tag != "Shield" || other.tag == "Shield" && !playerRocket) {
                Explode();
            }
            collided = true;
            if (other.tag == "Shield") {
                collided = false;
            }
        }
    }

    IEnumerator selfDestruct()
    {
        yield return new WaitForSeconds(3);
        // have no idea why but rocket wouldn't destroy itself if I put Destroy() after Explode()
        // also don't know why it didn't Destroy() when explode was called considering its a part of it
        Destroy(gameObject);
        Explode();
    }

    void Explode() {
        CreateChild.CreateChildPrefab(explosion, ParticleParent, transform, Random.rotation);
        Vector3 explosionPosition = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, radius);
            foreach (Collider hit in colliders) {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null && hit.tag != "StaysKinematic" && hit.GetComponent<BoxCollider>()) {
                    hit.GetComponent<BoxCollider>().isTrigger = true;
                    rb.isKinematic = false;
                    rb.AddExplosionForce(power, explosionPosition, radius, 0, ForceMode.Impulse);
                    Destroy(gameObject);
                }
            }
        closeToPlayer = false;
        Destroy(gameObject);
    }
}
