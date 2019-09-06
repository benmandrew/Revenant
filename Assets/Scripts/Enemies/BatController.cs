using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatController : MonoBehaviour, ActorInterface {

    bool free;
    bool isProtectedByShield;
    float health;
    static float MAXHEALTH = 150f;
    float speed;

    GameObject player;
    Rigidbody2D rb2d;
    GameObject body;
    GameObject sprite;

    public int positionInRenderQueue;
    float arenaRadius;

    bool turningToPlayer;
    bool turningFromWall;
    float turningSpeed; // Degrees per second
    float angleDifferenceThreshold;
    float maxPerpendicularDistance;

    Quaternion constDirection;

    // Use this for initialization
    void Awake () {
        free = true;
        isProtectedByShield = false;

        speed = 1500.0f;
        player = GameObject.FindGameObjectWithTag("Player");
        rb2d = GetComponent<Rigidbody2D>();
        body = transform.GetChild(0).gameObject;
        sprite = body.transform.GetChild(0).gameObject;

        turningToPlayer = false;
        turningFromWall = false;
        turningSpeed = 100f * Mathf.Deg2Rad;
        angleDifferenceThreshold = 10f;
        maxPerpendicularDistance = 6f;

        constDirection = transform.rotation;
        arenaRadius = GameObject.FindGameObjectWithTag("Walls").GetComponent<WallsController>().getRadius();
    }
	
	// Update is called once per frame
	void Update () {
        rb2d.velocity = Vector2.zero;
        if (!free) {
            manageMovement();
            if (health <= 0.0f) {
                freeActor();
            }
        }
	}

    void manageMovement() {
        rb2d.angularVelocity = 0.0f;
        if (turningFromWall = willCollideWithWall()) {
            turnFromWall();
        } else if (getPerpendicularDistanceToPlayer() < -maxPerpendicularDistance) {
            turningToPlayer = true;
            turnToPlayer();
        } else if (isFacingPlayer()) {
            constDirection = transform.rotation;
            turningToPlayer = false;
        }
        if (!turningFromWall && !turningToPlayer) {
            transform.rotation = constDirection;
        }
        move();
    }

    float getPerpendicularDistanceToPlayer() {
        float distance = Vector3.Magnitude(transform.position - player.transform.position);
        float angle = Vector3.Angle(transform.right, player.transform.position - transform.position);
        return distance * Mathf.Cos(angle * Mathf.Deg2Rad);
    }

    bool isFacingPlayer() {
        Vector3 difference = (player.transform.position - transform.position).normalized;
        return (Vector3.Angle(transform.right, difference) < angleDifferenceThreshold);
    }

    void turnToPlayer() {
        Vector2 toPlayer = player.transform.position - transform.position;
        float angle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, turningSpeed * Time.deltaTime);
        //transform.Rotate(Vector3.forward, turningSpeed * Time.deltaTime);
    }

    void turnFromWall() {
        // angle to center of arena
        float angle = Mathf.Atan2(-transform.position.y, -transform.position.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, turningSpeed * Time.deltaTime);
    }

    void move() {
        Vector3 velocity = transform.right * speed * Time.deltaTime;
        rb2d.velocity = velocity;
    }

    bool willCollideWithWall() {
        float buffer = 10f;
        return transform.position.magnitude + buffer > arenaRadius;
    }

    public void createActor(GameObject parent) {
        health = MAXHEALTH;
        sprite.GetComponent<SpriteRenderer>().enabled = true;
        sprite.GetComponent<Animator>().enabled = true;
        body.GetComponent<Collider2D>().enabled = true;
        free = false;
    }

    public void freeActor() {
        rb2d.velocity = Vector2.zero;
        sprite.GetComponent<SpriteRenderer>().enabled = false;
        sprite.GetComponent<Animator>().enabled = false;
        body.GetComponent<Collider2D>().enabled = false;
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
        SpriteRenderer spriteRend = sprite.GetComponent<SpriteRenderer>();
        spriteRend.sortingOrder += position;
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
