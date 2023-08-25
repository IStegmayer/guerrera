using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dzajna {
// TODO: I'll get back to this when I do AI 
public class EnemyStatsManager : CharacterStatsManager {
    [SerializeField] private UIEnemyHealthBar healthBar;
    public int SoulsReward;
    public bool IsBoss;
    private EnemyManager enemyManager;
    private CharacterAnimatorManager enemyAnim;
    private EnemyBossManager enemyBossManager;

    private Coroutine deathRoutine;

    private void Awake() {
        enemyManager = GetComponent<EnemyManager>();
        enemyBossManager = GetComponent<EnemyBossManager>();
        maxHealth = SetMaxHealthFromHealthLevel();
        maxFP = SetMaxFPAmountFromFocusLevel();
        currentFP = maxFP;
        currentHealth = maxHealth;
    }

    private void Start() {
        if (!IsBoss) healthBar.SetMaxHealth(maxHealth);
        enemyAnim = enemyManager.CharacterAnimatorManager;
    }

    protected override void HandlePoiseResetTimer() {
        if (PoiseResetTimer > 0) PoiseResetTimer = PoiseResetTimer - Time.deltaTime;
        else if (!enemyManager.IsInteracting) TotalPoise = ArmorPoise;
    }

    private int SetMaxFPAmountFromFocusLevel() {
        return focusLevel * focusPerLevel;
    }

    private int SetMaxHealthFromHealthLevel() {
        return healthLevel * healthPerLevel;
    }

    public override void TakeDamageWithoutAnimation(float dmg) {
        if (enemyManager.IsDead) return;
        if (enemyManager.IsInvulnerable && !enemyManager.IsBeingRiposted && !enemyManager.IsBeingBackstabbed) return;

        currentHealth -= dmg;
        if (IsBoss && enemyBossManager != null) enemyBossManager.UpdateBossHealthBar(currentHealth, maxHealth);
        else if (healthBar != null) healthBar.SetCurrentHealth(currentHealth);

        if (currentHealth <= 0) {
            deathRoutine = StartCoroutine(DeathRoutine());
            enemyManager.IsDead = true;
        }
    }

    public override void TakeDamage(float dmg, int dmgAnimation = -1) {
        if (enemyManager.IsDead || enemyManager.IsInvulnerable) return; 
        if (dmgAnimation == -1) dmgAnimation = enemyAnim.Damage1Hash;

        currentHealth -= dmg;
        if (IsBoss && enemyBossManager != null) {
            enemyBossManager.UpdateBossHealthBar(currentHealth, maxHealth);
        }
        else if (healthBar != null) healthBar.SetCurrentHealth(currentHealth);

        if (enemyManager.IsPhaseShifting) return;
        
        if (currentHealth <= 0) {
            //TODO: holy shit
            enemyAnim.PlayTargetAnimation(enemyAnim.DeathHash, true);
            deathRoutine = StartCoroutine(DeathRoutine());
            enemyManager.IsDead = true;
        }
        else {
            if (enemyManager.CharacterStatsManager.TotalPoise > 0) enemyAnim.PlayTargetAnimation(dmgAnimation, false);
            else enemyManager.Stagger(dmgAnimation);
        }
    }

    private IEnumerator DeathRoutine() {
        yield return new WaitForSeconds(1f);
        //TODO: This could be a bitch. 
        GameManager.Instance.ChangeState(GameState.Win);
        yield return new WaitForSeconds(5f);
        UnitManager.Instance.DespawnEnemy(transform.gameObject);
    }

    public float GetMaxHealth() => maxHealth;
}
}