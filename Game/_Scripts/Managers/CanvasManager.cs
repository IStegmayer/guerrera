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

    private Stack<GameObject> canvasStack = new Stack<GameObject>();

    //TODO: Use a stack for this
    private void Start() {
        canvasStack.Push(menuCanvas);
    }

    public void LoadGameScene(string sceneName) {
        if (canvasStack.Count > 0) canvasStack.Pop().SetActive(false);
        loadingCanvas.SetActive(true);
        canvasStack.Push(loadingCanvas);

        StartCoroutine(LoadSceneAsync(sceneName));
    }

    public void NavigateToCanvas(GameObject targetCanvas) {
        if (canvasStack.TryPeek(out GameObject activeCanvas)) activeCanvas.SetActive(false);
        if (activeCanvas == targetCanvas) {
            canvasStack.Pop();
            return;
        }

        canvasStack.Push(targetCanvas);
        targetCanvas.SetActive(true);
    }
    
    public void NavigateBack() {
        canvasStack.TryPop(out var currentCanvas);
        canvasStack.TryPeek(out var targetCanvas);
        if (currentCanvas != null) currentCanvas.SetActive(false);
        if (targetCanvas != null) targetCanvas.SetActive(true);
    }


    public void TryToOpenInGameOptions() {
        canvasStack.TryPeek(out var activeCanvas);
        if (activeCanvas == menuCanvas) return;
        
        if (activeCanvas != null) NavigateBack();
        else NavigateToCanvas(inGameOptionsCanvas);
    }

    private IEnumerator LoadSceneAsync(string sceneName) {
        AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName);

        while (!scene.isDone) {
            progressBar.fillAmount = Mathf.Clamp01(scene.progress / 0.9f);
            yield return null;
        }

        canvasStack.Pop();
        loadingCanvas.SetActive(false);
        canvasStack.Clear();
        
        //TODO: use an enum
        if (sceneName == "MainMenuScene") {
            canvasStack.Push(menuCanvas);
            menuCanvas.SetActive(true);
        }

        yield return null;
    }
}
}