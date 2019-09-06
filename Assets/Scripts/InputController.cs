using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {

    KeyCode moveUpKey;
    KeyCode moveLeftKey;
    KeyCode moveDownKey;
    KeyCode moveRightKey;
    KeyCode shootKey;
    KeyCode powerupKey;
    KeyCode pickUpItemKey;
    
    PlayerController playerController;

    // Use this for initialization
    void Start () {
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        PlayerPrefs.SetInt("MoveUp", (int)KeyCode.W);
        PlayerPrefs.SetInt("MoveLeft", (int)KeyCode.A);
        PlayerPrefs.SetInt("MoveDown", (int)KeyCode.S);
        PlayerPrefs.SetInt("MoveRight", (int)KeyCode.D);
        PlayerPrefs.SetInt("Shoot", (int)KeyCode.Mouse0);
        PlayerPrefs.SetInt("ActivatePowerup", (int)KeyCode.Space);
        PlayerPrefs.SetInt("PickUpItem", (int)KeyCode.E);

        moveUpKey = (KeyCode)PlayerPrefs.GetInt("MoveUp");
        moveLeftKey = (KeyCode)PlayerPrefs.GetInt("MoveLeft");
        moveDownKey = (KeyCode)PlayerPrefs.GetInt("MoveDown");
        moveRightKey = (KeyCode)PlayerPrefs.GetInt("MoveRight");
        shootKey = (KeyCode)PlayerPrefs.GetInt("Shoot");
        powerupKey = (KeyCode)PlayerPrefs.GetInt("ActivatePowerup");
        pickUpItemKey = (KeyCode)PlayerPrefs.GetInt("PickUpItem");
    }
	
	// Update is called once per frame
	void Update () {
        if (playerController.isAlive) {
            if (Input.GetKey(moveUpKey)) {
                playerController.moveUp();
            }
            if (Input.GetKey(moveLeftKey)) {
                playerController.moveLeft();
            }
            if (Input.GetKey(moveDownKey)) {
                playerController.moveDown();
            }
            if (Input.GetKey(moveRightKey)) {
                playerController.moveRight();
            }
            if (Input.GetKey(shootKey)) {
                playerController.shoot();
            }
            if (Input.GetKey(powerupKey)) {
                playerController.activatePowerup();
            }
            if (Input.GetKey(pickUpItemKey)) {
                playerController.pickUpItem();
            }
        }
    }
}
