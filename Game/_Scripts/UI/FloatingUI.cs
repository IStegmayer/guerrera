using System;
using UnityEngine;
using UnityEngine.UI;

namespace Dzajna {
public class FloatingUI : MonoBehaviour {
    [SerializeField] private RawImage lockOn;
    [SerializeField] private RawImage backstab;
    [SerializeField] private PlayerCombatManager playerCombatManager;
    private Camera mainCamera;
    private PlayerCamera privateCamera;

    private void Start() {
        mainCamera = Camera.main;
        privateCamera = PlayerCamera.Instance;
    }

    private void Update() {
        bool canCrit = playerCombatManager.GetCanBackstab() || playerCombatManager.GetCanRiposte();
        bool isLockedOn = privateCamera.CurrentLockOnTarget != null;
        
        if (canCrit) {
            Vector3 pos =
                mainCamera.WorldToScreenPoint(playerCombatManager.GetEnemyCharacter().LockOnTransform.position);
            backstab.gameObject.SetActive(true);
            lockOn.gameObject.SetActive(false);
            if (pos != backstab.transform.position) backstab.transform.position = pos;
        }
        else if (isLockedOn) {
            Vector3 pos =
                mainCamera.WorldToScreenPoint(privateCamera.CurrentLockOnTarget.LockOnTransform.position);
            backstab.gameObject.SetActive(false);
            lockOn.gameObject.SetActive(true);
            if (pos != lockOn.transform.position) lockOn.transform.position = pos;
        }
        else {
            lockOn.gameObject.SetActive(false);
            backstab.gameObject.SetActive(false);
        }
    }
}
}