using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {

    private bool loadScene;
    public string loadingSceneName;
    public Text loadingText;
    public Slider sliderBar;

    void Start() {
        loadScene = false;
        sliderBar.gameObject.SetActive(false);
        loadingText.gameObject.SetActive(false);
    }

    public void quit() {
        Application.Quit();
    }

    public void loadGame() {
        if (loadScene == false) {
            loadScene = true;
            //sliderBar.gameObject.SetActive(true); // Done in Inspector
            loadingText.text = "Loading...";
            StartCoroutine(loadNewScene(loadingSceneName));
        }
    }

    IEnumerator loadNewScene(string sceneName) {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        while (!async.isDone) {
            float progress = Mathf.Clamp01(async.progress / 0.9f);
            sliderBar.value = progress;
            loadingText.text = progress * 100f + "%";
            yield return null;
        }
    }
}
