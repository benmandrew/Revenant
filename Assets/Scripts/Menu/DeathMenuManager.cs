using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenuManager : MonoBehaviour {

    PlayerController playerController;
    WaveController waveController;
    GameObject uiObject;
    GameObject deathMenuObject;

    void Start() {
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        waveController = GameObject.FindGameObjectWithTag("Wave Manager").GetComponent<WaveController>();
        uiObject = GameObject.FindGameObjectWithTag("UI");
        deathMenuObject = GameObject.FindGameObjectWithTag("DeathMenu");
        gameObject.SetActive(false);
    }

	void Update () {
		if (Input.GetKey(KeyCode.Space)) {
            uiObject.SetActive(true);
            deathMenuObject.SetActive(false);
            waveController.initialise();
            playerController.initialise();
        }
        if (Input.GetKey(KeyCode.Escape)) {
            SceneManager.LoadScene("Menu");
        }
	}
}
