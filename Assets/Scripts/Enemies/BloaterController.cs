using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloaterController : DamnedController {

    float cloudRadius;
    bool isCloud;
    float cloudDecayTime;
    float currentDecayTime;

    float bloaterRadius;

    Animator anim;

    WaveController waveController;

    // Use this for initialization
    void Awake() {
        free = true;
        isProtectedByShield = false;
        speed = 600.0f;
        player = GameObject.FindGameObjectWithTag("Player");
        rb2d = GetComponent<Rigidbody2D>();
        body = transform.GetChild(0).gameObject;

        arenaRadius = GameObject.FindGameObjectWithTag("Walls").GetComponent<WallsController>().getRadius();

        cloudRadius = 8f;
        cloudDecayTime = 5f;
        bloaterRadius = 0.8f;
        anim = body.GetComponent<Animator>();
        waveController = GameObject.FindGameObjectWithTag("Wave Manager").GetComponent<WaveController>();
    }

    // Update is called once per frame
    void Update() {
        rb2d.angularVelocity = 0f;
        if (!free) {
            if (!isCloud) {
                moveTowardsPlayer();
                facePlayer();
            } else { 
                currentDecayTime += Time.deltaTime;
            }

            if (health <= 0.0f && !isCloud) {
                becomeCloud();
                anim.SetTrigger("Become Cloud");
                isCloud = true;
            }
            // Permanent clouds until wave over
            else if (/*currentDecayTime > cloudDecayTime ||*/ waveController.allStagesDead()) {
                currentDecayTime = 0f;
                isCloud = false;
                anim.SetTrigger("Reset");
                resetToBloater();
                freeActor();
            }
        }
    }
    
    void becomeCloud() {
        rb2d.velocity = Vector2.zero;
        rb2d.angularVelocity = 0f;
        rb2d.isKinematic = true;
        CircleCollider2D col = body.GetComponent<CircleCollider2D>();
        col.radius = cloudRadius;
        col.isTrigger = true;
        gameObject.tag = "Cloud";

        SpriteRenderer sprite = body.GetComponent<SpriteRenderer>();
        sprite.sortingLayerName = "Clouds";
    }

    void resetToBloater() {
        rb2d.isKinematic = false;
        CircleCollider2D col = body.GetComponent<CircleCollider2D>();
        col.radius = bloaterRadius;
        col.isTrigger = false;
        gameObject.tag = "Enemy";

        SpriteRenderer sprite = body.GetComponent<SpriteRenderer>();
        sprite.sortingLayerName = "Enemies";
    }

    bool allOtherStagesDead() {
        List<ActorPoolController> actorPools = waveController.getActorPools();
        foreach (ActorPoolController actorPool in actorPools) {
            if (actorPool != null) {
                if (actorPool.getBoundActorNum() != 0
                    && actorPool.getActorType().GetType() != typeof(BloaterController)) {
                    return false;
                }
            }
        }
        return true;
    }
}
