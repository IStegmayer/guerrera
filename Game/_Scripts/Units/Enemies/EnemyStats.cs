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
        // private PlayerAnimatorManager playerAnimatorManager;
        private Animator animator;

        private Coroutine deathRoutine;

        private void Awake() {
            animator = GetComponent<Animator>();
            // playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
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

        public void TakeDamage(int _dmg) {
            if (isDead) return;
            currentHealth -= _dmg;
            // healthBar.SetCurrentHealth(currentHealth);
            animator.Play("Damage1");
            
            if (currentHealth <= 0) {
                deathRoutine = StartCoroutine(DeathRoutine());
                isDead = true;
            }
        }
        
        private IEnumerator DeathRoutine() {
            // start death animation
            animator.Play("Death");
            yield return new WaitForSeconds(1f);
            //TODO: This could be a bitch. 
            GameManager.Instance.ChangeState(GameState.Win);
            yield return new WaitForSeconds(1f);
            UnitManager.Instance.DespawnPlayer(transform.gameObject);
        }
    }
    
}