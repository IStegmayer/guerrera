using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

namespace Dzajna {
public class DamageCollider : MonoBehaviour {
    public CharacterManager characterManager;
    public bool EnabledOnStartup;

    protected Collider damageCollider;
    [Header("Damage")]
    public float CurrentWeaponDamage = 25;

    [Header("Poise")] 
    public float PoiseBreak = 10;
    public float OffensivePoiseBonus = 10;

    private void Awake() {
        damageCollider = GetComponent<Collider>();
        damageCollider.gameObject.SetActive(true);
        damageCollider.isTrigger = true;
        damageCollider.enabled = EnabledOnStartup;
    }

    public void EnableDamageCollider() => damageCollider.enabled = true;
    public void DisableDamageCollider() => damageCollider.enabled = false;

    private void OnTriggerEnter(Collider collider) {
        //TODO: dont use a tag for this, maybe an interface?
        
        if (collider.gameObject.CompareTag("Player")) {
            PlayerStats playerStats = collider.GetComponent<PlayerStats>();
            CharacterManager enemyCharacterManager = collider.GetComponent<CharacterManager>();
            BlockingCollider shield = collider.GetComponentInChildren<BlockingCollider>(true);
            AnimatorManager anim = characterManager.GetComponent<AnimatorManager>();
            CharacterEffectsManager charEffectsManager = collider.GetComponent<CharacterEffectsManager>();
            float dmgToBeDealt = CurrentWeaponDamage;

            if (enemyCharacterManager == null) throw new Exception("No enemy character manager found.");
            if (playerStats == null) throw new Exception("No player stats manager found.");

            if (enemyCharacterManager.IsParrying) {
                // CHECK IF YOU ARE PARRIABLE
                anim.PlayTargetAnimation(anim.ParriedHash, true);
                return;
            }

            if (shield != null && enemyCharacterManager.IsBlocking) {
                dmgToBeDealt = CurrentWeaponDamage -
                                (CurrentWeaponDamage * shield.blockingPhysicalDmgAbsorption) / 100;
            }

            string dmgAnim = enemyCharacterManager.IsBlocking ? "BlockDamage1" : "Damage1";
            Vector3 contactPoint =
                collider.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            charEffectsManager.PlayBloodSplatterFX(contactPoint);
            playerStats.ResetPoiseTimer();
            playerStats.DealPoiseDamage(PoiseBreak);
            if (playerStats.TotalPoise > PoiseBreak) {
                playerStats.TakeDamageWithoutAnimation(CurrentWeaponDamage);
            }
            else playerStats.TakeDamage(dmgToBeDealt, dmgAnim);
        }

        if (collider.gameObject.CompareTag("Enemy")) {
            //TODO: add blocking
            EnemyStatsManager enemyStatsManager = collider.GetComponent<EnemyStatsManager>();
            CharacterManager enemyCharacterManager = collider.GetComponent<CharacterManager>();
            CharacterEffectsManager charEffectsManager = collider.GetComponent<CharacterEffectsManager>();
            if (enemyCharacterManager != null && enemyCharacterManager.IsParrying) {
                // CHECK IF YOU ARE PARRIABLE
                AnimatorManager anim = characterManager.GetComponent<AnimatorManager>();
                anim.PlayTargetAnimation(anim.ParriedHash, true);
                return;
            }

            if (enemyStatsManager != null) {
                Vector3 contactPoint =
                    collider.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                charEffectsManager.PlayBloodSplatterFX(contactPoint);
                enemyStatsManager.ResetPoiseTimer();
                enemyStatsManager.DealPoiseDamage(PoiseBreak);
                if (enemyStatsManager.TotalPoise > PoiseBreak) enemyStatsManager.TakeDamage(CurrentWeaponDamage);
                else {
                    enemyStatsManager.TakeDamageWithoutAnimation(CurrentWeaponDamage);
                    enemyStatsManager.BreakGuard();
                }
            }
        }
    }
}
}