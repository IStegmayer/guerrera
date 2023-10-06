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
    private Collider damageCollider;

    [Header("Damage")] //
    public float PhysicalDamage;

    public float FireDamage;

    [Header("Poise")] //
    public float PoiseBreak;

    public float OffensivePoiseBonus = 10;

    [Header("Guard Break Modifier")] //
    public float GuardBreakModifier = 1;

    private readonly List<CharacterManager> charactersDamagedDuringThisCalculation = new List<CharacterManager>();
    protected int currentDamageAnimation;
    protected bool hasBeenParried;
    protected bool shieldHasBeenHit;


    private void Awake() {
        damageCollider = GetComponent<Collider>();
        damageCollider.gameObject.SetActive(true);
        damageCollider.isTrigger = true;
        damageCollider.enabled = EnabledOnStartup;
    }

    public void EnableDamageCollider() => damageCollider.enabled = true;

    public void DisableDamageCollider() {
        damageCollider.enabled = false;
        charactersDamagedDuringThisCalculation.Clear();
    }


    private void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("DamageableCharacter")) {
            shieldHasBeenHit = false;
            hasBeenParried = false;

            CharacterManager enemyManager = collision.gameObject.GetComponentInParent<CharacterManager>();
            if (enemyManager == null) throw new Exception("No enemy character manager found.");

            if (enemyManager.IsBeingBackstabbed || enemyManager.IsBeingRiposted) {
                characterManager.CharacterEffectsManager.SetLastHitPosition(
                    collision.ClosestPointOnBounds(transform.position));
                EnsureAggroTarget(enemyManager);
                return;
            }

            // TODO: check for teamID to not hit
            if (enemyManager == characterManager || enemyManager.TeamID == characterManager.TeamID) return;
            if (charactersDamagedDuringThisCalculation.Contains(enemyManager)) return;
            charactersDamagedDuringThisCalculation.Add(enemyManager);

            CheckForBlock(enemyManager);
            CheckForParry(enemyManager);
            if (shieldHasBeenHit || hasBeenParried) return;

            CharacterEffectsManager enemyEffectsManager = enemyManager.CharacterEffectsManager;
            CharacterStatsManager enemyStats = enemyManager.CharacterStatsManager;
            //TODO: this is not necessary anymore
            Vector3 contactPoint = collision.ClosestPointOnBounds(transform.position);

            // TODO: this should draw the modifier from the scriptableObject
            float attackTypeMultiplier;
            if (characterManager.CharacterCombatManager.CurrentAttackType == AttackType.Heavy)
                attackTypeMultiplier = 2f;
            else attackTypeMultiplier = 1f;
            
            float directionHitFrom = Vector3.SignedAngle(characterManager.transform.forward,
                enemyManager.transform.forward, Vector3.up);
            ChooseWhichDirectionDamageCameFrom(directionHitFrom, enemyManager);

            enemyStats.DealPoiseDamage(PoiseBreak * attackTypeMultiplier);
            enemyStats.TakeDamage(PhysicalDamage * attackTypeMultiplier, currentDamageAnimation);
            enemyStats.ResetPoiseTimer();
            if (!enemyManager.IsInvulnerable) enemyEffectsManager.PlayBloodSplatterFX(contactPoint);

            EnsureAggroTarget(enemyManager);
        }
    }

    private void EnsureAggroTarget(CharacterManager enemyManager) {
        var aiCharacter = enemyManager as EnemyManager;
        if (aiCharacter != null && characterManager.GetType() != typeof(EnemyManager))
            aiCharacter.CurrentTarget = characterManager;
    }

    protected virtual void CheckForParry(CharacterManager enemyManager) {
        if (!enemyManager.IsParrying) return;

        var charAnim = characterManager.CharacterAnimatorManager;
        charAnim.PlayTargetAnimation(charAnim.ParriedHash, true);
        enemyManager.CharacterSfxManager.PlayParrySound();
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
        enemyManager.CharacterSfxManager.PlayBlockSound();
        var physicalDamageAfterBlock =
            PhysicalDamage - PhysicalDamage * enemyShield.BlockingPhysicalDamageAbsorption / 100;

        enemyManager.CharacterCombatManager.AttemptBlock(this, PhysicalDamage, FireDamage, "Block_01");
        enemyShield.TakeDamageWithoutAnimation(physicalDamageAfterBlock);
    }

    protected virtual void ChooseWhichDirectionDamageCameFrom(float direction, CharacterManager enemyManager) {
        if (direction >= 145 && direction <= 180)
            currentDamageAnimation = enemyManager.CharacterAnimatorManager.StaggerFrontHash;
        else if (direction <= -145 && direction >= -180)
            currentDamageAnimation = enemyManager.CharacterAnimatorManager.StaggerFrontHash;
        else if (direction >= -45 && direction <= 45)
            currentDamageAnimation = enemyManager.CharacterAnimatorManager.StaggerBackHash;
        else if (direction >= -144 && direction <= -45)
            currentDamageAnimation = enemyManager.CharacterAnimatorManager.StaggerLeftHash;
        else if (direction >= 45 && direction <= 144)
            currentDamageAnimation = enemyManager.CharacterAnimatorManager.StaggerRightHash;
    }
}
}