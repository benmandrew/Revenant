using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperController : MonoBehaviour, ActorInterface {

    bool free;
    bool isProtectedByShield;
    float health;
    static float MAXHEALTH = 400f;

    GameObject player;
    Rigidbody2D rb2d;
    GameObject body;
    Animator anim;

    BulletPoolController fireBallPool;

    public int positionInRenderQueue;
    float arenaRadius;
    float boundsLength;

    bool fire;
    bool teleport;

    float teleportRadiusMin;
    float teleportRadiusMax;
    
    float firePrepareTime;
    float fireRecoverTime;
    float teleportPrepareTime;
    float teleportRecoverTime;

    float timeSinceLastChange;

    // Use this for initialization
    void Awake() {
        free = true;
        isProtectedByShield = false;
        player = GameObject.FindGameObjectWithTag("Player");
        rb2d = GetComponent<Rigidbody2D>();
        body = transform.GetChild(0).gameObject;
        anim = body.GetComponent<Animator>();
        anim.SetTrigger("Teleport");

        fireBallPool = GameObject.Find("FireBallPool").GetComponent<BulletPoolController>();

        fire = false;
        teleport = false;

        teleportRadiusMax = 20;
        teleportRadiusMin = 16;
        timeSinceLastChange = 0f;

        arenaRadius = GameObject.FindGameObjectWithTag("Walls").GetComponent<WallsController>().getRadius();
        boundsLength = body.GetComponent<SpriteRenderer>().bounds.extents.magnitude;
    }
    
    // Update is called once per frame
    void Update() {
        rb2d.velocity = Vector2.zero;
        if (!free) {
            manageFiring();
            manageMovement();
            facePlayer();
            timeSinceLastChange += Time.deltaTime;
            if (health <= 0.0f) {
                freeActor();
            }
        }
    }

    // Animator event functions
    public void setFire() {
        fire = true;
        anim.SetTrigger("Teleport");
    }

    public void setTeleport() {
        teleport = true;
        anim.SetTrigger("Fire");
    }
    // ------------------------

    void manageFiring() {
        if (fire) {
            fireBallPool.createBullet(gameObject, "FireBall");
            fire = false;
        }
    }

    void manageMovement() {
        if (teleport) {
            Vector3 teleportCoords = calcTeleportCoords();
            transform.position = teleportCoords;
            teleport = false;
        }
    }

    Vector3 calcTeleportCoords() {
        Vector3 newCoords;
        // Keep generating new coordinates if it's outside the arena
        do {
            Vector3 playerCoords = player.transform.position;
            float angle = Random.Range(0, 2 * Mathf.PI);
            float radius = Random.Range(teleportRadiusMin, teleportRadiusMax);
            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);
            newCoords = playerCoords + new Vector3(x, y, 0);
        } while (newCoords.magnitude > arenaRadius - boundsLength);
        return newCoords;
    }

    void facePlayer() {
        Vector2 difference = player.transform.position - transform.position;
        float angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
    }

    public void createActor(GameObject parent) {
        health = MAXHEALTH;
        body.GetComponent<SpriteRenderer>().enabled = true;
        body.GetComponent<Collider2D>().enabled = true;
        body.GetComponent<Animator>().enabled = true;
        free = false;

        transform.position = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);
    }

    public void freeActor() {
        rb2d.velocity = Vector2.zero;
        body.GetComponent<SpriteRenderer>().enabled = false;
        body.GetComponent<Collider2D>().enabled = false;
        body.GetComponent<Animator>().enabled = false;
        free = true;
    }

    public bool isFree() {
        return free;
    }

    public bool isProtected() {
        return isProtectedByShield;
    }

    public void setProtected(bool isProtected) {
        isProtectedByShield = isProtected;
    }

    public void setPosition(Vector3 newPosition) {
        transform.position = newPosition;
    }

    public void setRenderingOrder(int position) {
        SpriteRenderer sprite = body.GetComponent<SpriteRenderer>();
        sprite.sortingOrder += position;
    }

    public float getHealth() {
        return health;
    }

    public void removeHealth(float damage) {
        if (!isProtectedByShield) {
            health -= damage;
        }
    }
}