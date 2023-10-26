using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dzajna {
public class BossHealthBar : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI bossName;
    [SerializeField] private GameObject healthBarGameObject;
    [SerializeField] private Slider slider;
    public void SetBossName(string name) => bossName.text = name;
    public void ToggleBossHealthBarActivation(bool val) => healthBarGameObject.SetActive(val);
    public void SetBossCurrentHealth(float currentHealth) => slider.value = currentHealth;

    public void SetBossMaxHealth(float maxHealth) {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }
}
}