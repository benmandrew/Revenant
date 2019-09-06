using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    Text pointsText;
    Text healthText;
    Text waveText;

    Image currentItemOverlay;
    Image powerupDamageItemOverlay;
    Image powerupSlowMotionItemOverlay;
    Image powerupInvincibilityItemOverlay;
    Image healthItemOverlay;

    RectTransform currentItemRectTransform;
    Vector3 currentItemWorldPos;
    Camera cam;

    // Use this for initialization
    void Start () {
        pointsText = transform.GetChild(0).GetComponent<Text>();
        healthText = transform.GetChild(1).GetComponent<Text>();
        waveText = transform.GetChild(2).GetComponent<Text>();

        powerupDamageItemOverlay = transform.GetChild(3).GetComponent<Image>();
        powerupSlowMotionItemOverlay = transform.GetChild(4).GetComponent<Image>();
        powerupInvincibilityItemOverlay = transform.GetChild(5).GetComponent<Image>();
        healthItemOverlay = transform.GetChild(6).GetComponent<Image>();

        currentItemOverlay = powerupDamageItemOverlay;
        currentItemRectTransform = currentItemOverlay.GetComponent<RectTransform>();
        cam = Camera.main;
    }

    void Update() {
        if (currentItemOverlay.enabled) {
            Vector2 screenPos = cam.WorldToScreenPoint(currentItemWorldPos);
            screenPos.x -= cam.pixelWidth / 2f;
            screenPos.y -= cam.pixelHeight / 2f;
            screenPos.x += 1.2f * (currentItemRectTransform.rect.width / 2f);
            screenPos.y += 1.2f * (currentItemRectTransform.rect.height / 2f);
            currentItemOverlay.rectTransform.anchoredPosition = screenPos;
        }
    }

    public void updatePointsText(int points) {
        pointsText.text = "Points: " + points.ToString();
    }

    public void updateHealthText(int healthPoints) {
        healthText.text = "Health: " + healthPoints.ToString();
    }

    public void updateWaveText(int wave) {
        waveText.text = "Wave " + wave.ToString();
    }

    public void updateItemOverlay(string type, Vector3 itemWorldPos) {
        currentItemWorldPos = itemWorldPos;
        switch (type) {
            case "Damage":
                currentItemOverlay = powerupDamageItemOverlay;
                break;
            case "SlowMotion":
                currentItemOverlay = powerupSlowMotionItemOverlay;
                break;
            case "Invincibility":
                currentItemOverlay = powerupInvincibilityItemOverlay;
                break;
            case "Health":
                currentItemOverlay = healthItemOverlay;
                break;
        }
    }

    public void enableItemOverlay() {
        currentItemOverlay.enabled = true;
    }

    public void disableItemOverlay() {
        currentItemOverlay.enabled = false;
    }
}
