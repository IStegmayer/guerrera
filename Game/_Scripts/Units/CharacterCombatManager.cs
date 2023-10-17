using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Dzajna {
public class CharacterCombatManager : MonoBehaviour {
    [SerializeField] protected Transform critRayCastStartPoint;
    [SerializeField] private Transform backStabberStandPoint;
    [SerializeField] private Transform riposterStandPoint;
    [SerializeField] protected float maxCritDistance = 1.5f;
    [SerializeField] private LayerMask characterLayer;
    public int LastAttackHash { get; set; }
    public AttackType CurrentAttackType { get; set; }

    private CharacterManager character;

    private bool canBackstab;
    private bool canRiposte;
    private CharacterManager enemyCharacter;

    protected virtual void Start() {
        character = GetComponent<CharacterManager>();
    }

    private void Update() {
        //TODO: at least format this
        if (character.IsPerformingBackstab || character.IsPerformingRiposte || !Physics.Raycast(
                critRayCastStartPoint.position, critRayCastStartPoint.forward,
                out RaycastHit hit, maxCritDistance, characterLayer)) {
            enemyCharacter = null;
            canRiposte = false;
            canBackstab = false;
            return;
        }

        enemyCharacter = hit.transform.GetComponent<CharacterManager>();
        var directionFromCharacterToEnemy = transform.position - enemyCharacter.transform.position;
        var dotValue = Vector3.Dot(directionFromCharacterToEnemy, enemyCharacter.transform.forward);

        //TODO: make these distances variables
        if (enemyCharacter.CanBeRiposted && dotValue <= 1.5f && dotValue >= 0.2f) {
            canRiposte = true;
            return;
        }
        else canRiposte = false;

        if (dotValue >= -1.5f && dotValue <= -0.2f) canBackstab = true;
        else canBackstab = false;
    }

    public void SetBlockingAbsorptionsFromBlockingWeapon() {
        CharacterStatsManager charStats = character.CharacterStatsManager;
        CharacterInventoryManager charInventory = character.CharacterInventoryManager;

        if (character.IsUsingRightHand) {
            charStats.BlockingPhysicalDamageAbsorption = charInventory.RightWeapon.PhysicalBlockingDamageAbsorption;
            charStats.BlockingFireDamageAbsorption = charInventory.RightWeapon.FireBlockingDamageAbsorption;
            charStats.BlockingStabilityRating = charInventory.RightWeapon.Stability;
        }
        else if (character.IsUsingLeftHand) {
            charStats.BlockingPhysicalDamageAbsorption = charInventory.LeftWeapon.PhysicalBlockingDamageAbsorption;
            charStats.BlockingFireDamageAbsorption = charInventory.LeftWeapon.FireBlockingDamageAbsorption;
            charStats.BlockingStabilityRating = charInventory.LeftWeapon.Stability;
        }
    }

    public bool AttemptBackStabOrRiposte() {
        if (canBackstab) {
            AttemptBackStab();
            return true;
        }

        if (canRiposte) {
            AttemptRiposte();
            return true;
        }

        return false;
    }

    private void AttemptBackStab() {
        if (enemyCharacter.IsBeingBackstabbed && enemyCharacter.IsBeingRiposted) return;

        var charAnim = character.CharacterAnimatorManager;
        var charInv = character.CharacterInventoryManager;

        //We make it so the enemy cannot be damaged whilst being critically damaged
        charAnim.EnableIsInvulnerable();
        // charAnim.EraseHandIKForWeapon();
        character.IsPerformingBackstab = true;

        charAnim.PlayTargetAnimation(charAnim.BackStabHash, true);

        float criticalDamage = charInv.RightWeapon.CriticalDamageMultiplier * charInv.RightWeapon.BaseDamage;
        enemyCharacter.PendingCriticalDamage = criticalDamage;
        enemyCharacter.CharacterCombatManager.GetBackStabbed(character, enemyCharacter.CharacterAnimatorManager);
    }

    private void AttemptRiposte() {
        if (enemyCharacter.IsBeingBackstabbed && enemyCharacter.IsBeingRiposted) return;

        var charAnim = character.CharacterAnimatorManager;
        var charInv = character.CharacterInventoryManager;

        //We make it so the enemy cannot be damaged whilst being critically damaged
        charAnim.EnableIsInvulnerable();
        // charAnim.EraseHandIKForWeapon();
        character.IsPerformingRiposte = true;
        enemyCharacter.IsBeingRiposted = true;

        charAnim.PlayTargetAnimation(charAnim.RiposteHash, true);

        float criticalDamage = charInv.RightWeapon.CriticalDamageMultiplier * charInv.RightWeapon.BaseDamage;
        enemyCharacter.PendingCriticalDamage = criticalDamage;
        enemyCharacter.CharacterCombatManager.GetRiposted(character, enemyCharacter.CharacterAnimatorManager);
    }

    private void GetBackStabbed(CharacterManager characterPerformingBackStab, CharacterAnimatorManager charAnim) {
        character.IsBeingBackstabbed = true;

        ForceMoveCharacterToCriticalPosition(characterPerformingBackStab, true);
        charAnim.PlayTargetAnimation(charAnim.BackStabbedHash, true, true);
    }

    private void GetRiposted(CharacterManager characterPerformingBackStab, CharacterAnimatorManager charAnim) {
        character.IsBeingRiposted = true;

        ForceMoveCharacterToCriticalPosition(characterPerformingBackStab, false);
        charAnim.PlayTargetAnimation(charAnim.RipostedHash, true);
    }

    private void ForceMoveCharacterToCriticalPosition(CharacterManager characterPerformingCrit, bool isBackstab) {
        characterPerformingCrit.CharacterController.enabled = false;

        Transform critPerformerTransform = characterPerformingCrit.transform;
        Transform spotTransform = isBackstab ? backStabberStandPoint : riposterStandPoint;
        critPerformerTransform.position = spotTransform.position;

        critPerformerTransform.LookAt(transform);

        characterPerformingCrit.CharacterController.enabled = true;
    }

    public void AttemptBlock(DamageCollider attackingWeapon, float physicalDamage, float fireDamage, string block) {
        var staminaDamageAbsorption = (physicalDamage + fireDamage)
                                      * attackingWeapon.GuardBreakModifier
                                      * (character.CharacterStatsManager.BlockingStabilityRating / 100);

        Debug.Log(staminaDamageAbsorption);

        var staminaDamage = (physicalDamage + fireDamage)
            * attackingWeapon.GuardBreakModifier - staminaDamageAbsorption;

        character.CharacterStatsManager.TakeStaminaDamage(staminaDamage);

        if (character.CharacterStatsManager.CheckHasStamina()) {
            character.IsBlocking = false;
            character.CharacterAnimatorManager.PlayTargetAnimation(character.CharacterAnimatorManager.ParriedHash,
                true);
        }
        else {
            character.CharacterAnimatorManager.PlayTargetAnimation(character.CharacterAnimatorManager.BlockHash, true);
        }
    }

    public void SuccessfullyCastSpell() {
        character.CharacterInventoryManager.CurrentSpell.SuccessfullyCastedSpell(character);
    }

    public bool GetCanBackstab() => canBackstab;
    public bool GetCanRiposte() => canRiposte;
    public CharacterManager GetEnemyCharacter() => enemyCharacter;
}
}