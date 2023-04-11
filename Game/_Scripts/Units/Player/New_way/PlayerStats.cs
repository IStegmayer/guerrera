using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dzajna {
public class PlayerStats : CharacterStats {
    private PlayerManager playerManager;
    private PlayerAnimatorManager playerAnimatorManager;

    private HealthBar healthBar;
    private StaminaBar staminaBar;
    private StatusBar FPBar;
    private Coroutine deathRoutine;

    public int SoulCount = 0;

    private void Awake() {
        playerManager = GetComponent<PlayerManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        //TODO: this could be better
        staminaBar = FindObjectOfType<StaminaBar>();
        healthBar = FindObjectOfType<HealthBar>();
        FPBar = FindObjectOfType<StatusBar>();
    }

    private void Start() {
        maxHealth = SetMaxHealthFromHealthLevel();
        healthBar.SetMaxHealth(maxHealth);
        currentHealth = maxHealth;

        maxStamina = SetMaxStaminaFromStaminaLevel();
        // set stamina max bar
        staminaBar.SetMaxStamina(maxStamina);
        currentStamina = maxStamina;

        maxFP = SetMaxFPAmountFromFocusLevel();
        FPBar.SetMaxStat(maxFP);
        currentFP = maxFP;
    }

    protected override void HandlePoiseResetTimer() {
        if (PoiseResetTimer > 0) PoiseResetTimer = PoiseResetTimer - Time.deltaTime;
        else if (!playerManager.IsInteracting) TotalPoise = ArmorPoise;
    }

    private int SetMaxHealthFromHealthLevel() {
        return healthLevel * healthPerLevel;
    }

    private int SetMaxStaminaFromStaminaLevel() {
        return staminaLevel * staminaPerLevel;
    }
    private int SetMaxFPAmountFromFocusLevel() {
        return focusLevel * focusPerLevel;
    }

    public bool CheckIfCanCast(int _FPCost) {
        return currentFP >= _FPCost;
    }

    public bool CheckHasStamina() => currentStamina >= 0;

    public override void TakeDamageWithoutAnimation(float _dmg) {
        if (IsDead || playerManager.IsInvulnerable) return;
        currentHealth -= _dmg;
            
        if (currentHealth <= 0) {
            deathRoutine = StartCoroutine(DeathRoutine());
            IsDead = true;
        }
    }

    public override void TakeDamage(float _dmg, string _dmgAnimation = "Damage1") {
        if (IsDead || playerManager.IsInvulnerable) return;
        
        currentHealth -= _dmg;
        healthBar.SetCurrentHealth(currentHealth);
        playerAnimatorManager.PlayTargetAnimation(_dmgAnimation, true);

        if (currentHealth <= 0) {
            playerAnimatorManager.PlayTargetAnimation(playerAnimatorManager.DeathHash, true);   
            deathRoutine = StartCoroutine(DeathRoutine());
            IsDead = true;
        }
    }

    private IEnumerator DeathRoutine() {
        // start death animation
        yield return new WaitForSeconds(1f);
        GameManager.Instance.ChangeState(GameState.Lose);
        yield return new WaitForSeconds(1f);
        UnitManager.Instance.DespawnPlayer(transform.gameObject);
    }

    public void TakeStaminaDamage(float _dmg) {
        currentStamina -= _dmg;
        staminaBar.SetCurrentStamina(currentStamina);
    }
    public void TakeFPDamage(float _dmg) {
        currentFP -= _dmg;
        FPBar.SetCurrentStat(currentFP);
    }

    public void RegenerateStamina() {
        if (currentStamina >= maxStamina) return;

        if (playerManager.IsInteracting || playerManager.IsSprinting) staminaRegenTimer = 0;
        else {
            staminaRegenTimer += Time.deltaTime;
            if (staminaRegenTimer > 1f) {
                currentStamina += staminaRegenAmount * Time.deltaTime;
                staminaBar.SetCurrentStamina(currentStamina);
            }
        }
    }

    public void AddSouls(int _souls) => SoulCount += _souls;
}
}