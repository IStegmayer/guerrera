using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dzajna {
public class PlayerStatsManager : CharacterStatsManager {
    private PlayerManager player;
    private PlayerCharacterAnimatorManager playerAnimManager;

    [SerializeField] private HealthBar healthBar;
    [SerializeField] private StaminaBar staminaBar;
    [SerializeField] private FPBar FPBar;
    private Coroutine deathRoutine;

    public int SoulCount;
    public static event Action OnPlayerKilled;

    private void Awake() {
        player = GetComponent<PlayerManager>();
        playerAnimManager = GetComponent<PlayerCharacterAnimatorManager>();
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
        FPBar.SetMaxFP(maxFP);
        currentFP = maxFP;
    }

    protected override void HandlePoiseResetTimer() {
        if (PoiseResetTimer > 0) PoiseResetTimer = PoiseResetTimer - Time.deltaTime;
        else if (!player.IsInteracting) TotalPoise = ArmorPoise;
    }

    private int SetMaxHealthFromHealthLevel() {
        return healthLevel * healthPerLevel;
    }

    private int SetMaxStaminaFromStaminaLevel() {
        return staminaLevel * staminaPerLevel;
    }

    private float SetMaxFPAmountFromFocusLevel() {
        return focusLevel * focusPerLevel;
    }

    public bool CheckIfCanCast(int FPCost) {
        return currentFP >= FPCost;
    }

    public override void TakeDamageWithoutAnimation(float dmg) {
        if (player.IsDead || player.IsInvulnerable) return;

        currentHealth -= dmg;
        healthBar.SetCurrentHealth(currentHealth);

        if (currentHealth <= 0) {
            deathRoutine = StartCoroutine(DeathRoutine());
            player.IsDead = true;
        }
    }

    public override void TakeDamage(float dmg, int dmgAnimation = -1) {
        if (player.IsDead || player.IsInvulnerable) return;
        if (dmgAnimation == -1) dmgAnimation = playerAnimManager.Damage1Hash;

        currentHealth -= dmg;
        healthBar.SetCurrentHealth(currentHealth);

        if (currentHealth <= 0) {
            deathRoutine = StartCoroutine(DeathRoutine());
            player.IsDead = true;
        }
        else {
            if (TotalPoise <= 0) player.Stagger(dmgAnimation);
            else if (!player.IsInteracting) playerAnimManager.PlayTargetAnimation(playerAnimManager.Damage1Hash, false);
        }
    }

    public override void TakeStaminaDamage(float dmg) {
        base.TakeStaminaDamage(dmg);
        staminaBar.SetCurrentStamina(currentStamina);
    }

    private IEnumerator DeathRoutine() {
        playerAnimManager.PlayTargetAnimation(playerAnimManager.DeathHash, true);
        // start death animation
        yield return new WaitForSeconds(3f);
        GameManager.Instance.ChangeState(GameState.Lose);
        OnPlayerKilled?.Invoke();
        yield return new WaitForSeconds(3f);
        UnitManager.Instance.DespawnPlayer(transform.gameObject);
        CanvasManager.Instance.LoadGameScene("GameScene");
    }

    public void TakeFPDamage(float dmg) {
        currentFP -= dmg;
        FPBar.SetCurrentFP(currentFP);
    }

    public void RegenerateStamina() {
        if (currentStamina >= maxStamina) return;

        if (player.IsInteracting || player.IsSprinting) staminaRegenTimer = 0;
        else {
            staminaRegenTimer += Time.deltaTime;
            if (staminaRegenTimer > staminaResetTime) {
                currentStamina += staminaRegenAmount * Time.deltaTime;
                staminaBar.SetCurrentStamina(currentStamina);
            }
        }
    }

    public void AddSouls(int souls) => SoulCount += souls;
}
}