using System;
using UnityEngine;

namespace Dzajna {
public class CharacterStats : MonoBehaviour {
    //TODO: ofc this is all simplified
    [SerializeField] protected int healthLevel = 10;
    [SerializeField] protected int healthPerLevel = 5;
    [SerializeField] protected int staminaLevel = 10;
    [SerializeField] protected int staminaPerLevel = 5;
    [SerializeField] protected float staminaRegenAmount = 2f;
    [SerializeField] protected int focusLevel = 10;
    [SerializeField] protected int focusPerLevel = 5;

    [Header("Poise")] public float TotalPoise;
    public float OffensivePoise;
    public float ArmorPoise;
    public float TotalPoiseResetTime = 5;
    public float PoiseResetTimer = 0;

    protected float maxHealth;
    protected float currentHealth;
    protected float maxStamina;
    protected float currentStamina;
    protected float maxFP;
    protected float currentFP;
    protected float staminaRegenTimer;
    public bool IsDead;

    public virtual void TakeDamage(float _dmg, string _dmgAnimation = "Damage1") { }
    public virtual void TakeDamageWithoutAnimation(float _dmg) { }

    protected void Update() {
        HandlePoiseResetTimer();
    }

    private void Start() {
        TotalPoise = ArmorPoise;
    }

    protected virtual void HandlePoiseResetTimer() { }

    public void ResetPoiseTimer() => PoiseResetTimer = TotalPoiseResetTime;
    public void DealPoiseDamage(float _dmg) => TotalPoise = TotalPoise - _dmg;
}
}