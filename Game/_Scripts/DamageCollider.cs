using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.TextCore.Text;

namespace Dzajna {
public class DamageCollider : MonoBehaviour {
    public CharacterManager characterManager;
    public bool EnabledOnStartup;

    protected Collider damageCollider;

    [Header("Damage")] //
    public float PhysicalDamage = 25;

    public float FireDamage;

    [Header("Poise")] //
    public float PoiseBreak = 10;

    public float OffensivePoiseBonus = 10;

    [Header("Guard Break Modifier")] //
    public float GuardBreakModifier = 1;

    private readonly List<CharacterManager> charactersDamagedDuringThisCalculation = new List<CharacterManager>();
    protected string currentDamageAnimation;
    protected bool hasBeenParried;
    protected bool shieldHasBeenHit;


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
            PlayerStatsManager playerStatsManager = collider.GetComponent<PlayerStatsManager>();
            CharacterManager enemyCharacterManager = collider.GetComponent<CharacterManager>();
            CharacterAnimatorManager anim = characterManager.GetComponent<CharacterAnimatorManager>();
            CharacterEffectsManager charEffectsManager = collider.GetComponent<CharacterEffectsManager>();
            float dmgToBeDealt = PhysicalDamage;

            if (enemyCharacterManager == null) throw new Exception("No enemy character manager found.");
            if (playerStatsManager == null) throw new Exception("No player stats manager found.");

            CheckForBlock(characterManager);
            CheckForParry(enemyCharacterManager, anim);
            if (shieldHasBeenHit || hasBeenParried) return;

            string dmgAnim = enemyCharacterManager.IsBlocking ? "BlockDamage1" : "Damage1";
            Vector3 contactPoint =
                collider.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            charEffectsManager.PlayBloodSplatterFX(contactPoint);
            playerStatsManager.ResetPoiseTimer();
            playerStatsManager.DealPoiseDamage(PoiseBreak);
            if (playerStatsManager.TotalPoise > PoiseBreak) {
                playerStatsManager.TakeDamageWithoutAnimation(PhysicalDamage);
            }
            else playerStatsManager.TakeDamage(dmgToBeDealt, dmgAnim);
        }

        if (collider.gameObject.CompareTag("Enemy")) {
            //TODO: add blocking
            EnemyStatsManagerManager enemyStatsManagerManager = collider.GetComponent<EnemyStatsManagerManager>();
            CharacterManager enemyCharacterManager = collider.GetComponent<CharacterManager>();
            CharacterEffectsManager charEffectsManager = collider.GetComponent<CharacterEffectsManager>();
            if (enemyCharacterManager != null && enemyCharacterManager.IsParrying) {
                // CHECK IF YOU ARE PARRIABLE
                CharacterAnimatorManager anim = characterManager.GetComponent<CharacterAnimatorManager>();
                anim.PlayTargetAnimation(anim.ParriedHash, true);
                return;
            }

            if (enemyStatsManagerManager != null) {
                Vector3 contactPoint =
                    collider.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                charEffectsManager.PlayBloodSplatterFX(contactPoint);
                enemyStatsManagerManager.ResetPoiseTimer();
                enemyStatsManagerManager.DealPoiseDamage(PoiseBreak);
                if (enemyStatsManagerManager.TotalPoise > PoiseBreak)
                    enemyStatsManagerManager.TakeDamage(PhysicalDamage);
                else {
                    enemyStatsManagerManager.TakeDamageWithoutAnimation(PhysicalDamage);
                    enemyStatsManagerManager.BreakGuard();
                }
            }
        }
    }

    protected virtual void CheckForParry(CharacterManager enemyManager, CharacterAnimatorManager enemyAnim) {
        if (!enemyManager.IsParrying) return;

        enemyAnim.PlayTargetAnimation(enemyAnim.ParriedHash, true);
        hasBeenParried = true;
    }

    protected virtual void CheckForBlock(CharacterManager enemyManager) {
        if (!enemyManager.IsBlocking) return;

        // Check if enemy is hitting from the front
        var enemyShield = enemyManager.CharacterStatsManager;
        var directionFromPlayerToEnemy = characterManager.transform.position - enemyManager.transform.position;
        var dotValueFromPlayerToEnemy = Vector3.Dot(directionFromPlayerToEnemy, enemyManager.transform.forward);
        if (dotValueFromPlayerToEnemy < 0.3f) return;
        
        shieldHasBeenHit = true;
        var physicalDamageAfterBlock =
            PhysicalDamage - PhysicalDamage * enemyShield.BlockingPhysicalDamageAbsorption / 100;
        
        enemyManager.CharacterCombatManager.AttemptBlock(this, PhysicalDamage, FireDamage, "Block_01");
        enemyShield.TakeDamage(physicalDamageAfterBlock);
    }
}
}