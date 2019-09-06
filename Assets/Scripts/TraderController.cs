using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraderController : MonoBehaviour {

    CircleCollider2D traderCollider;
    Animator anim;
    SpriteRenderer rend;

    bool isActive;
    bool activating;
    bool deactivating;

    bool canPickupPowerup;
    bool canPickupHealth;

    WaveController waveController;
    ItemPoolController itemPool;
    UIController uiController;
    PlayerController player;

    CircleCollider2D powerupCollider;
    SpriteRenderer powerupSpriteRend;
    CircleCollider2D healthCollider;
    SpriteRenderer healthSpriteRend;

    Sprite damagePowerupSprite;
    Sprite slowMotionPowerupSprite;
    Sprite invincibilityPowerupSprite;
    //Sprite healthSprite;

    // Use this for initialization
    void Start () {
        traderCollider = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();

        waveController = GameObject.FindGameObjectWithTag("Wave Manager").GetComponent<WaveController>();
        itemPool = GameObject.FindGameObjectWithTag("ItemPool").GetComponent<ItemPoolController>();
        uiController = GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        Transform powerup = transform.GetChild(0);
        powerupCollider = powerup.GetComponent<CircleCollider2D>();
        powerupSpriteRend = powerup.GetComponent<SpriteRenderer>();

        Transform health = transform.GetChild(1);
        healthCollider = health.GetComponent<CircleCollider2D>();
        healthSpriteRend = health.GetComponent<SpriteRenderer>();
        healthSpriteRend.sprite = Resources.Load<Sprite>("Sprites/Items/potion_red");

        damagePowerupSprite = Resources.Load<Sprite>("Sprites/Items/potion_purple");
        slowMotionPowerupSprite = Resources.Load<Sprite>("Sprites/Items/potion_blue");
        invincibilityPowerupSprite = Resources.Load<Sprite>("Sprites/Items/potion_yellow");

        canPickupPowerup = false;
        canPickupHealth = false;
        isActive = false;
        deactivate();
	}
    
    void generateNewItems() {
        string type = itemPool.createPowerup();
        switch (type) {
            case "Damage":
                setPowerupSprite(damagePowerupSprite);
                break;
            case "SlowMotion":
                setPowerupSprite(slowMotionPowerupSprite);
                break;
            case "Invincibility":
                setPowerupSprite(invincibilityPowerupSprite);
                break;
        }
    }

    void setPowerupSprite(Sprite sprite) {
        powerupSpriteRend.sprite = sprite;
    }

    public void removePowerupSprite() {
        //setPowerupSprite(null);
        powerupSpriteRend.enabled = false;
        uiController.disableItemOverlay();
    }

    public bool getActive() {
        return isActive;
    }

    public bool isActivating() {
        return activating;
    }

    public bool isDeactivating() {
        return deactivating;
    }

    public bool isAlive() {
        return getActive() || isActivating() || isDeactivating();
    }

    public void startActivation() {
        if (!isActive) {
            isActive = true;
            activating = true;
            rend.enabled = true;
            generateNewItems();
            powerupSpriteRend.enabled = true;
            healthSpriteRend.enabled = true;
            anim.SetTrigger("Appear");
        }
    }

    public void startDeactivation() {
        if (isActive) {
            isActive = false;
            deactivating = true;
            anim.SetTrigger("Disappear");
        }
    }

    public void activate() {
        traderCollider.enabled = true;
        activating = false;
    }

    public void deactivate() {
        traderCollider.enabled = false;
        rend.enabled = false;
        powerupSpriteRend.enabled = false;
        healthSpriteRend.enabled = false;
        uiController.disableItemOverlay();
        deactivating = false;
        canPickupPowerup = false;
        canPickupHealth = false;
        waveController.newWave = true;
    }
    
    public bool getPlayerCanPickupPowerup() {
        return canPickupPowerup && itemPool.getBackupPowerup().getType() != "Empty";
    }

    public bool getPlayerCanPickupHealth() {
        return canPickupHealth && player.getHealth() < PlayerController.MAXHEALTH;
    }

    public void OnCollisionEnter2DPowerup(Collider2D col) {
        if (itemPool.getBackupPowerup().getType() != "Empty") {
            canPickupPowerup = true;
            string type = itemPool.getBackupPowerup().getType();
            Vector3 itemWorldPos = transform.GetChild(0).position;
            uiController.updateItemOverlay(type, itemWorldPos);
            // Enable overlay must come after update overlay (you're a DOLT)
            uiController.enableItemOverlay();
        }
    }

    public void OnCollisionExit2DPowerup(Collider2D col) {
        canPickupPowerup = false;
        uiController.disableItemOverlay();
    }

    public void OnCollisionEnter2DHealth(Collider2D col) {
        canPickupHealth = true;
        Vector3 itemWorldPos = transform.GetChild(1).position;
        uiController.updateItemOverlay("Health", itemWorldPos);
        // Enable overlay must come after update overlay (you're a DOLT)
        uiController.enableItemOverlay();
    }

    public void OnCollisionExit2DHealth(Collider2D col) {
        canPickupHealth = false;
        uiController.disableItemOverlay();
    }
}
