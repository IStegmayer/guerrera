using System;
using UnityEngine;
using UnityEngine.UI;

namespace Dzajna {
public class FloatingUI : MonoBehaviour {
    [SerializeField] private RawImage lockOn;
    private Camera mainCamera;
    private PlayerCamera privateCamera;

    private void Start() {
        mainCamera = Camera.main;
        privateCamera = PlayerCamera.Instance;
    }

    private void Update() {
        if (privateCamera.CurrentLockOnTarget == null) {
            lockOn.gameObject.SetActive(false);
            return;
        }
        
        Vector3 pos =
            mainCamera.WorldToScreenPoint(privateCamera.CurrentLockOnTarget.LockOnTransform.position);

        lockOn.gameObject.SetActive(true);
        if (pos != lockOn.transform.position) lockOn.transform.position = pos;
    }
}
}