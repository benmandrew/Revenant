using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHelper : MonoBehaviour {
    void quit() {
        Application.Quit();
    }

    void startGame() {
        SceneManager.LoadScene("Game");
    }
}
