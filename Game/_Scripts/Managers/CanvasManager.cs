using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Dzajna {
public class CanvasManager : PersistentSingleton<CanvasManager> {
    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private GameObject controlsCanvas;
    [SerializeField] private GameObject inGameOptionsCanvas;
    [SerializeField] private Image progressBar;

    private GameObject activeCanvas;
    private GameObject lastActiveCanvas;

    //TODO: Use a stack for this
    private void Start() {
        activeCanvas = menuCanvas;
    }

    public void LoadGameScene(string sceneName) {
        if (activeCanvas != null) activeCanvas.SetActive(false);
        loadingCanvas.SetActive(true);

        StartCoroutine(LoadSceneAsync(sceneName));
    }

    public void ActivateCanvas(GameObject canvasToActivate) {
        if (activeCanvas == canvasToActivate) {
            ActivateLastCanvas();
            return;
        }

        if (activeCanvas != null) activeCanvas.SetActive(false);
        canvasToActivate.SetActive(true);
        lastActiveCanvas = activeCanvas;
        activeCanvas = canvasToActivate;
    }

    public void ActivateLastCanvas() {
        activeCanvas.SetActive(false);
        activeCanvas = lastActiveCanvas;

        if (lastActiveCanvas == null) return;

        lastActiveCanvas.SetActive(true);
        lastActiveCanvas = null;
    }

    public void ShowMainMenu() {
        activeCanvas.SetActive(false);
        menuCanvas.SetActive(true);
        lastActiveCanvas = activeCanvas;
        activeCanvas = menuCanvas;
    }

    public GameObject GetInGameOptionCanvas() => inGameOptionsCanvas;

    private IEnumerator LoadSceneAsync(string sceneName) {
        AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName);

        while (!scene.isDone) {
            progressBar.fillAmount = Mathf.Clamp01(scene.progress / 0.9f);
            yield return null;
        }

        //TODO: use an enum
        if (sceneName == "MainMenuScene") {
            activeCanvas = menuCanvas;
            activeCanvas.SetActive(true);
        }
        else if (activeCanvas != null) {
            activeCanvas.SetActive(false);
            activeCanvas = null;
        }

        loadingCanvas.SetActive(false);
        yield return null;
    }
}
}