using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

    GameObject bulletPrefab;
    GameObject parent;
    Collider2D parentCollider;
    Rigidbody2D rb2d;

    BulletPoolController bulletContainer;

    bool free = true;
    bool parentIsPlayer;

    float deleteRadius;

    string parentWeapon;

	// Use this for initialization
	void Awake () {
        rb2d = GetComponent<Rigidbody2D>();
        free = true;
        GetComponent<SpriteRenderer>().enabled = false;
        deleteRadius = GameObject.FindGameObjectWithTag("Walls").GetComponent<WallsController>().getRadius();
    }
	
    public void setParent(GameObject newParent) {
        parent = newParent;
        parentIsPlayer = parent.GetComponent<PlayerController>() != null;
        parentCollider = parent.transform.GetChild(0).gameObject.GetComponent<Collider2D>();
    }

    public void setBulletContainer(BulletPoolController newBulletContainer) {
        bulletContainer = newBulletContainer;
    }

	// Update is called once per frame
	void Update () {
        if (isOutsideDeleteRadius() && !free) {
            freeBullet();
        }
	}

    bool isOutsideDeleteRadius() {
        float xSqr = Mathf.Pow(transform.position.x, 2);
        float ySqr = Mathf.Pow(transform.position.y, 2);
        return (xSqr + ySqr) > Mathf.Pow(deleteRadius, 2);
    }



    public void resetToParent() {
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<CircleCollider2D>().enabled = true;
        free = false;

        transform.position = parent.transform.position;
        transform.rotation = parent.transform.rotation;

        float angle = getParentAngle();
        setAngle(angle - 90);
        Vector2 newVelocity = new Vector2(
            Mathf.Cos(Mathf.Deg2Rad * angle) * bulletContainer.getBulletSpeed(),
            Mathf.Sin(Mathf.Deg2Rad * angle) * bulletContainer.getBulletSpeed());

        rb2d.velocity = newVelocity;
    }
    
    float getParentAngle() {
        if (parentIsPlayer) {
            return parent.GetComponent<PlayerController>().transform.GetChild(0).transform.eulerAngles.z;
        }
        return parent.transform.eulerAngles.z;
    }

    void setAngle(float angle) {
        Vector3 rot = Vector3.zero;
        rot.z = angle;
        transform.eulerAngles = rot;
    }

    public void setParentWeapon(string _parentWeapon) {
        parentWeapon = _parentWeapon;
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.transform.parent != null) {
            if (free
                || col == parentCollider
                || col.transform.parent.tag == "NonCollidable"
                || col.transform.parent.tag == "Cloud") {
                return;
            } 
            
            else if (parent.tag == "Player"
                  && col.gameObject.transform.parent.tag == "Enemy") {
                playerEnemyCollision(col);
            } else if (parent.tag == "Enemy"
                && col.gameObject.transform.parent.tag == "Player") {
                enemyPlayerCollision(col);
            }
        } else {
            if (free
                || col == parentCollider) {
                return;
            }

            else if (parent.tag == "Player"
                && col.tag == "Trader") {
                col.gameObject.GetComponent<TraderController>().startDeactivation();
            }
        }
    }
    
    void playerEnemyCollision(Collider2D col) {
        ActorInterface enemy = col.transform.parent.GetComponent<ActorInterface>();
        enemy.removeHealth(bulletContainer.getBulletDamage());
        if (enemy.getHealth() <= 0) {
            parent.GetComponent<PlayerController>().killedEnemy(parentWeapon);
        }
        freeBullet();
    }

    void enemyPlayerCollision(Collider2D col) {
        PlayerController player = col.transform.parent.GetComponent<PlayerController>();
        player.damage();
        freeBullet();
    }



    public void freeBullet() {
        rb2d.velocity = Vector2.zero;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        free = true;
    }

    public bool isFree() {
        return free;
    }
}
