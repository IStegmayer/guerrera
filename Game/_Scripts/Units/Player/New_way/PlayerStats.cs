using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dzajna {
public class PlayerStats : CharacterStats {
    private PlayerManager playerManager;

    private HealthBar healthBar;
    private StaminaBar staminaBar;
    private StatusBar FPBar;
    private PlayerAnimatorManager playerAnimatorManager;

    private Coroutine deathRoutine;

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

    public void TakeDamage(int _dmg) {
        if (playerManager.IsInvulnerable) return;
        if (isDead) return;
        currentHealth -= _dmg;
        healthBar.SetCurrentHealth(currentHealth);
        playerAnimatorManager.PlayTargetAnimation(playerAnimatorManager.Damage1Hash, true);

        if (currentHealth <= 0) {
            deathRoutine = StartCoroutine(DeathRoutine());
            isDead = true;
        }
    }

    private IEnumerator DeathRoutine() {
        // start death animation
        playerAnimatorManager.PlayTargetAnimation(playerAnimatorManager.DeathHash, true);
        yield return new WaitForSeconds(1f);
        GameManager.Instance.ChangeState(GameState.Lose);
        yield return new WaitForSeconds(1f);
        UnitManager.Instance.DespawnPlayer(transform.gameObject);
    }

    public void TakeStaminaDamage(int _dmg) {
        currentStamina -= _dmg;
        staminaBar.SetCurrentStamina(currentStamina);
    }
    public void TakeFPDamage(int _dmg) {
        currentFP -= _dmg;
        FPBar.SetCurrentStat(currentFP);
    }

    public void RegenerateStamina() {
        if (currentStamina >= maxStamina) return;

        if (playerManager.IsInteracting) staminaRegenTimer = 0;
        else {
            staminaRegenTimer += Time.deltaTime;
            if (staminaRegenTimer > 1f) {
                currentStamina += staminaRegenAmount * Time.deltaTime;
                staminaBar.SetCurrentStamina(currentStamina);
            }
        }

    }
}
}