using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Dzajna {
public class UIEnemyHealthBar : MonoBehaviour {
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI poiseText;
    [SerializeField] private EnemyStatsManager statsManager;
    private float timeUntilBarIsHidden;
    private PlayerCamera playerCamera;

    private void Awake() {
        statsManager = GetComponentInParent<EnemyStatsManager>();
    }

    private void Start() {
        playerCamera = PlayerCamera.Instance;
    }

    public void SetCurrentHealth(float health) {
        slider.value = health;
        timeUntilBarIsHidden = 3f;
        if (!slider.gameObject.activeInHierarchy) slider.gameObject.SetActive(true);
    }

    public void SetMaxHealth(float maxHealth) {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }

    private void Update() {
        if (slider == null) return;
        transform.rotation = playerCamera.GetCameraRotation();
        // poiseText.text = statsManager.TotalPoise + "/" + statsManager.ArmorPoise;
        // TODO: this will tank performance for no reason
        // poiseText.text = GetComponentInParent<EnemyManager>().CurrentState.name;
        
        if (timeUntilBarIsHidden <= 0) {
            timeUntilBarIsHidden = 0;
            slider.gameObject.SetActive(false);
        }
        else timeUntilBarIsHidden = timeUntilBarIsHidden - Time.deltaTime;
        
        if (slider.value <= 0) Destroy(slider.gameObject);
    }
}
}