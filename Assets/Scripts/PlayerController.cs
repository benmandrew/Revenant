using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    float speed;
    Rigidbody2D rb2d;

    public static int MAXHEALTH = 3;
    int healthPoints;
    float timeSinceLastHealthLoss;
    float invulnerabilityTime;
    int points;

    bool isInvincible;
    float internalTimeScale;

    float shootCooldownTime;
    float timeSinceShot;
    bool hasShot;

    bool isVerticalInput;
    bool isHorizontalInput;
    bool isMoving;
    public bool isAlive;

    GameObject torso;
    GameObject reticle;
    GameObject legs;

    Animator torsoAnimator;
    Animator legsAnimator;

    WaveController waveController;
    TraderController trader;
    ItemPoolController itemPool;
    PowerUp powerup;

    BulletPoolController bulletPool;
    UIController ui;
    GameObject uiObject;
    GameObject deathMenuObject;

    Camera mainCam;

    // Use this for initialization
    void Start () {
        healthPoints = MAXHEALTH;
        timeSinceLastHealthLoss = 0f;
        invulnerabilityTime = 1f;
        points = 0;
        hasShot = false;
        isVerticalInput = false;
        isHorizontalInput = false;
        isMoving = false;
        isAlive = true;

        torso = transform.GetChild(0).gameObject;
        reticle = transform.GetChild(1).gameObject;
        legs = transform.GetChild(2).gameObject;

        torsoAnimator = torso.GetComponent<Animator>();
        legsAnimator = legs.GetComponent<Animator>();

        waveController = GameObject.FindGameObjectWithTag("Wave Manager").GetComponent<WaveController>();
        trader = GameObject.FindGameObjectWithTag("Trader").GetComponent<TraderController>();
        itemPool = GameObject.FindGameObjectWithTag("ItemPool").GetComponent<ItemPoolController>();
        powerup = itemPool.getMainPowerup();

        bulletPool = GameObject.FindGameObjectWithTag("BulletPool").GetComponent<BulletPoolController>();
        speed = 800f;
        healthPoints = 3;
        rb2d = GetComponent<Rigidbody2D>();
        uiObject = GameObject.FindGameObjectWithTag("UI");
        ui = uiObject.GetComponent<UIController>();
        deathMenuObject = GameObject.FindGameObjectWithTag("DeathMenu");

        internalTimeScale = 1f;

        shootCooldownTime = 0.02f;
        timeSinceShot = shootCooldownTime;

        mainCam = Camera.main;

        transform.position = new Vector3(0, 0, 0);
    }

    public void initialise() {
        Start();
        ui.updateHealthText(healthPoints);
    }

    public int getHealth() {
        return healthPoints;
    }

	// Update is called once per frame
	void Update () {
        resetVelocity();
        if (isAlive) {
            faceMouse();
            turnLegs();
            positionReticle();
            updateAnimator();
            if (getHealth() <= 0) {
                die();
            }
            timeSinceShot += Time.deltaTime;
            timeSinceLastHealthLoss += Time.deltaTime;
        }
        positionCamera();
        if (Input.GetKey(KeyCode.Escape)) {
            if (Time.timeScale == 1.0f) {
                Time.timeScale = 0.0f;
                Time.fixedDeltaTime = 0.0f;
            } else {
                Time.timeScale = 1.0f;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
            }
        }
    }

    public void moveUp() {
        if (isAlive) {
            Vector2 velocity = rb2d.velocity;
            velocity.y = speed * Time.deltaTime * internalTimeScale;
            rb2d.velocity = velocity;
            isVerticalInput = true;
            normaliseVelocity();
        }
    }

    public void moveLeft() {
        if (isAlive) {
            Vector2 velocity = rb2d.velocity;
            velocity.x = -speed * Time.deltaTime * internalTimeScale;
            rb2d.velocity = velocity;
            isHorizontalInput = true;
            normaliseVelocity();
        }
    }

    public void moveDown() {
        if (isAlive) {
            Vector2 velocity = rb2d.velocity;
            velocity.y = -speed * Time.deltaTime * internalTimeScale;
            rb2d.velocity = velocity;
            isVerticalInput = true;
            normaliseVelocity();
        }
    }

    public void moveRight() {
        if (isAlive) {
            Vector2 velocity = rb2d.velocity;
            velocity.x = speed * Time.deltaTime * internalTimeScale;
            rb2d.velocity = velocity;
            isHorizontalInput = true;
            normaliseVelocity();
        }
    }

    void normaliseVelocity() {
        if (isHorizontalInput && isVerticalInput) {
            rb2d.velocity /= Mathf.Sqrt(2f);
        }
    }

    public void shoot() {
        if (timeSinceShot < shootCooldownTime) {
            return;
        }
        bulletPool.createBullet(gameObject, "Rifle");
        timeSinceShot = 0.0f;
        hasShot = true;
    }

    public void activatePowerup() {
        if (powerup.getType() != "Empty") {
            powerup.activate();
        }
    }

    public void pickUpItem() {
        if (trader.getPlayerCanPickupPowerup()) {
            powerup = itemPool.pickUpPowerup();
            powerup.setHidden(false);
            trader.removePowerupSprite();
        }

        else if (trader.getPlayerCanPickupHealth()) {
            healthPoints = 3;
            ui.updateHealthText(healthPoints);
            trader.startDeactivation();
        }
    }

    void resetVelocity() {
        Vector2 velocity = rb2d.velocity;
        if (!isVerticalInput) {
            velocity.y = 0.0f;
        }
        if (!isHorizontalInput) {
            velocity.x = 0.0f;
        }

        if (isVerticalInput || isHorizontalInput) {
            isMoving = true;
        } else {
            isMoving = false;
        }

        rb2d.velocity = velocity;
        isVerticalInput = false;
        isHorizontalInput = false;
    }

    void faceMouse() {
        Vector2 mousePos = Input.mousePosition;
        Vector2 torsoPos = Camera.main.WorldToScreenPoint(torso.transform.position);
        Vector2 difference = new Vector3(
            mousePos.x - torsoPos.x,
            mousePos.y - torsoPos.y);
        float angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        torso.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void turnLegs() {
        float angle = Mathf.Atan2(rb2d.velocity.y, rb2d.velocity.x) * Mathf.Rad2Deg;
        legs.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void positionReticle() {
        reticle.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void positionCamera() {
        Vector3 newCamPosition = mainCam.transform.position;
        // Interpolate between the player and reticle
        float xInterp = (reticle.transform.position.x - transform.position.x) * 0.2f;
        float yInterp = (reticle.transform.position.y - transform.position.y) * 0.2f;
        newCamPosition.x = transform.position.x + xInterp;
        newCamPosition.y = transform.position.y + yInterp;
        mainCam.transform.position = newCamPosition;
    }

    void die() {
        isAlive = false;
        waveController.isPlaying = false;
        uiObject.SetActive(false);
        deathMenuObject.SetActive(true);
    }

    void updateAnimator() {
        legsAnimator.SetBool("Is Moving", isMoving);

        if (hasShot) {
            torsoAnimator.SetTrigger("Shoot Trigger");
            hasShot = false;
        }
    }

    public void killedEnemy(string weapon) {
        points += 10;
        ui.updatePointsText(points);
    }

    public void damage() {
        if (!isInvincible) {
            healthPoints--;
            ui.updateHealthText(healthPoints);
            timeSinceLastHealthLoss = 0;
        }
    }

    public void setInvincible(bool invincible) {
        isInvincible = invincible;
    }

    public void setTimeScale(float timeScale) {
        internalTimeScale = timeScale;
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "Enemy" && timeSinceLastHealthLoss > invulnerabilityTime) {
            damage();
        }
    }

    void OnTriggerStay2D(Collider2D col) {
        if (col.transform.parent.tag == "Cloud" && timeSinceLastHealthLoss > invulnerabilityTime) {
            damage();
        }
    }
}
