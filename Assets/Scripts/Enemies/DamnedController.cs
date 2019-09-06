using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamnedController : MonoBehaviour, ActorInterface {

    // Protected for use by Bloater class
    protected bool free;
    protected bool isProtectedByShield;
    protected float health;
    static float MAXHEALTH = 200f;
    protected float speed;

    protected GameObject player;
    protected Rigidbody2D rb2d;
    protected GameObject body;

    public int positionInRenderQueue;
    protected float arenaRadius;

    // Use this for initialization
    void Awake () {
        free = true;
        isProtectedByShield = false;
        speed = 600.0f;
        player = GameObject.FindGameObjectWithTag("Player");
        rb2d = GetComponent<Rigidbody2D>();
        body = transform.GetChild(0).gameObject;

        arenaRadius = GameObject.FindGameObjectWithTag("Walls").GetComponent<WallsController>().getRadius();

    }

	// Update is called once per frame
	void Update () {
        rb2d.velocity = Vector2.zero;
        if (!free) {
            moveTowardsPlayer();
            facePlayer();
            if (health <= 0.0f) {
                freeActor();
            }
        }
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

    // Protected for use by Bloater class
    protected void moveTowardsPlayer() {
        Vector2 velocity = (player.transform.position - transform.position).normalized * speed * Time.deltaTime;
        rb2d.velocity = velocity;
    }

    // Protected for use by Bloater class
    protected void facePlayer() {
        Vector2 difference = player.transform.position - transform.position;
        float angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
    }

    /*
    float eulerAnglesBetween(Quaternion from, Quaternion to) {
        Vector3 delta = to.eulerAngles - from.eulerAngles;
        if (delta.z > 180)
            delta.z -= 360;
        else if (delta.z < -180)
            delta.z += 360;
        return delta.z;
    }
    */

    Vector2 getVelocity() {
        float angle = (body.transform.rotation.eulerAngles.z - 90) * Mathf.Deg2Rad;
        float x = Mathf.Sin(angle);
        float y = -Mathf.Cos(angle);
        return new Vector2(x, y);
    }

    void OnCollisionEnter2D(Collision2D col) {

    }
}
