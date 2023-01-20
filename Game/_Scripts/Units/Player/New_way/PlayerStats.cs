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
        [SerializeField] private int staminaLevel = 10;
        [SerializeField] private int staminaPerLevel = 5;
        private int maxHealth;
        private int currentHealth;
        private int maxStamina;
        private int currentStamina;

        private HealthBar healthBar;
        private StaminaBar staminaBar;
        private AnimatorHandler animatorHandler;

        private Coroutine deathRoutine;

        private void Awake() {
            animatorHandler = GetComponent<AnimatorHandler>();
            //TODO: this could be better
            staminaBar = FindObjectOfType<StaminaBar>();
            healthBar = FindObjectOfType<HealthBar>();
        }

        private void Start() {
            maxHealth = SetMaxHealthFromHealthLevel();
            healthBar.SetMaxHealth(maxHealth);
            currentHealth = maxHealth;

            maxStamina = SetMaxStaminaFromStaminaLevel();
            // set stamina max bar
            staminaBar.SetMaxStamina(maxStamina);
            currentStamina = maxStamina;
        }

        private int SetMaxHealthFromHealthLevel() {
            return healthLevel * healthPerLevel;
        }

        private int SetMaxStaminaFromStaminaLevel() {
            return staminaLevel * staminaPerLevel;
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

        public void TakeStaminaDamage(int _dmg) {
            currentStamina -= _dmg;
            staminaBar.SetCurrentStamina(currentStamina);
        }
    }
    
}
