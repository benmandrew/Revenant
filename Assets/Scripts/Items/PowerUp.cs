using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PowerUp : ItemInterface {
    
    string type;
    float activationTime;
    float baseActivationTime;
    bool isActivated;
    bool isHidden; // The powerup, once used, is hidden so it can't be used until bought again

    PlayerController player;
    BulletPoolController bulletPool;
    Camera cam;

    float baseDamage;
    float damageMultiplier;

    float slowTimeScale;

    PostProcessingController postProcessor;

	// Use this for initialization
	public PowerUp () {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        bulletPool = GameObject.FindGameObjectWithTag("BulletPool").GetComponent<BulletPoolController>();
        cam = Camera.main;

        type = "Empty";
        activationTime = 5f;
        baseActivationTime = activationTime;
        isActivated = false;
        isHidden = true;

        baseDamage = bulletPool.getBulletDamage();
        damageMultiplier = 4f;

        slowTimeScale = 0.5f;

        postProcessor = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PostProcessingController>();
    }

    public void activate() {
        if (!isActivated && !isHidden && type != "Empty") {
            switch (type) {
                case "Damage":
                    activateDamage();
                    break;
                case "SlowMotion":
                    activateSlowMotion();
                    break;
                case "Invincibility":
                    activateInvincibility();
                    break;
            }
            isActivated = true;
            isHidden = true;
            setType("Empty");
        }
    }
    
    public void setHidden(bool hidden) {
        isHidden = hidden;
    }

    void activateDamage() {
        postProcessor.enableDamagePowerupPostProcessing();
        bulletPool.setBulletDamage(baseDamage * damageMultiplier);
        IEnumerator coroutine = deactivateDamageLate(activationTime);
        // Run the coroutine off of the player's MonoBehaviour
        player.StartCoroutine(coroutine);
    }

    IEnumerator deactivateDamageLate(float time) {
        yield return new WaitForSeconds(time);
        postProcessor.disableDamagePowerupPostProcessing();
        bulletPool.setBulletDamage(baseDamage);
        isActivated = false;
    }

    void activateSlowMotion() {
        postProcessor.enableSlowMotionPowerupPostProcessing();
        Time.timeScale = slowTimeScale;
        player.setTimeScale(1f / slowTimeScale);
        Time.fixedDeltaTime = 0.02f * slowTimeScale;
        activationTime *= slowTimeScale;
        IEnumerator coroutine = deactivateSlowMotionLate(activationTime);
        // Run the coroutine off of the player's MonoBehaviour
        player.StartCoroutine(coroutine);
    }

    IEnumerator deactivateSlowMotionLate(float time) {
        yield return new WaitForSeconds(time);
        postProcessor.disableSlowMotionPowerupPostProcessing();
        Time.timeScale = 1f;
        player.setTimeScale(1f);
        Time.fixedDeltaTime = 0.02f;
        activationTime = baseActivationTime;
        isActivated = false;
    }

    void activateInvincibility() {
        postProcessor.enableInvincibilityPowerupPostProcessing();
        player.setInvincible(true);
        IEnumerator coroutine = deactivateInvincibilityLate(activationTime);
        // Run the coroutine off of the player's MonoBehaviour
        player.StartCoroutine(coroutine);
    }

    IEnumerator deactivateInvincibilityLate(float time) {
        yield return new WaitForSeconds(time);
        postProcessor.disableInvincibilityPowerupPostProcessing();
        player.setInvincible(false);
        isActivated = false;
    }

    public string getType() {
        return type;
    }

    public void setType(string newType) {
        type = newType;
    }
}
