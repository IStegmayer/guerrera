using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dzajna {
// TODO: I'll get back to this when I do AI 
public class EnemyStats : CharacterStats {
    [SerializeField] private UIEnemyHealthBar healthBar;
    public int SoulsReward;
    public bool IsBoss;
    private EnemyAnimatorManager enemyAnimatorManager;
    private EnemyBossManager enemyBossManager;

    private Coroutine deathRoutine;

    private void Awake() {
        enemyAnimatorManager = GetComponent<EnemyAnimatorManager>();
        enemyBossManager = GetComponent<EnemyBossManager>();
        maxHealth = SetMaxHealthFromHealthLevel();
        currentHealth = maxHealth;
    }

    private void Start() {
        if (!IsBoss) healthBar.SetMaxHealth(maxHealth);
    }

    private int SetMaxHealthFromHealthLevel() {
        return healthLevel * healthPerLevel;
    }

    public void TakeDamageWithoutAnimation(float _dmg) {
        if (IsDead) return;
        currentHealth -= _dmg;
        if (IsBoss && enemyBossManager != null) enemyBossManager.UpdateBossHealthBar(currentHealth);
        else if (healthBar != null) healthBar.SetCurrentHealth(currentHealth);

        if (currentHealth <= 0) {
            deathRoutine = StartCoroutine(DeathRoutine());
            IsDead = true;
        }
    }

    public override void TakeDamage(float _dmg, string _dmgAnimation = "Damage1") {
        if (IsDead) return;
        currentHealth -= _dmg;
        if (IsBoss && enemyBossManager != null) enemyBossManager.UpdateBossHealthBar(currentHealth);
        else if (healthBar != null) healthBar.SetCurrentHealth(currentHealth);
        
        enemyAnimatorManager.PlayTargetAnimation(_dmgAnimation, true);

        if (currentHealth <= 0) {
            enemyAnimatorManager.PlayTargetAnimation("Death", true);
            deathRoutine = StartCoroutine(DeathRoutine());
            IsDead = true;
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