using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dzajna
{
    public class PlayerStats : MonoBehaviour
    {
        //TODO: ofc this is all simplified
        [SerializeField] private int healthLevel = 10;
        [SerializeField] private int healthPerLevel = 5;
        private int maxHealth;
        private int currentHealth;

        private HealthBar healthBar;
        private AnimatorHandler animatorHandler;

        private Coroutine deathRoutine;

        //TODO: this is NOT good
        private void Awake() {
            animatorHandler = GetComponent<AnimatorHandler>();
            healthBar = FindObjectOfType<HealthBar>();
        }

        private void Start() {
            maxHealth = SetMaxHealthFromHealthLevel();
            healthBar.SetMaxHealth(maxHealth);
            currentHealth = maxHealth;
        }

        private int SetMaxHealthFromHealthLevel() {
            return healthLevel * healthPerLevel;
        }

        public void TakeDamage(int _dmg) {
            currentHealth -= _dmg;
            healthBar.SetCurrentHealth(currentHealth);
            animatorHandler.PlayTargetAnimation(animatorHandler.Damage1Hash, true);
            
            if (currentHealth <= 0) {
                deathRoutine = StartCoroutine(DeathRoutine());
            }
        }
        private IEnumerator DeathRoutine() {
            // start death animation
            animatorHandler.PlayTargetAnimation(animatorHandler.DeathHash, true);
            yield return new WaitForSeconds(1f);
            GameManager.Instance.ChangeState(GameState.Lose);
            yield return new WaitForSeconds(1f);
            UnitManager.Instance.DespawnPlayer(transform.gameObject);
        }
    }
    
}
