using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dzajna
{
    // TODO: I'll get back to this when I do AI 
    public class EnemyStats : CharacterStats
    {

        // private HealthBar healthBar;
        private EnemyAnimatorManager enemyAnimatorManager;

        private Coroutine deathRoutine;

        private void Awake() {
            enemyAnimatorManager = GetComponent<EnemyAnimatorManager>();
            //TODO: this could be better
            // healthBar = FindObjectOfType<HealthBar>();
        }

        private void Start() {
            maxHealth = SetMaxHealthFromHealthLevel();
            // healthBar.SetMaxHealth(maxHealth);
            currentHealth = maxHealth;
        }

        private int SetMaxHealthFromHealthLevel() {
            return healthLevel * healthPerLevel;
        }

        public void TakeDamageWithoutAnimation(float _dmg) {
            if (IsDead) return;
            currentHealth -= _dmg;
            
            if (currentHealth <= 0) {
                deathRoutine = StartCoroutine(DeathRoutine());
                IsDead = true;
            }
        }

        public void TakeDamage(float _dmg) {
            if (IsDead) return;
            currentHealth -= _dmg;
            // healthBar.SetCurrentHealth(currentHealth);
            enemyAnimatorManager.PlayTargetAnimation("Damage1", true);
            
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
    }
    
}