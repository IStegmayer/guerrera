using System.Collections;
using System.Collections.Generic;
using Dzajna;
using UnityEngine;

public class ChangeSceneButton : MonoBehaviour {
    public void ChangeScene(string sceneName) {
        CanvasManager.Instance.LoadGameScene(sceneName);
    }
}