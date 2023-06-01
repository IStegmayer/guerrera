using System;
using UnityEngine;

namespace Dzajna {
public class CharacterCombatManager : MonoBehaviour {
    protected CharacterManager character;
    public int LastAttackHash;
    public AttackType CurrentAttackType;

    protected virtual void Start() {
        character = GetComponent<CharacterManager>();
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
            character.CharacterAnimatorManager.PlayTargetAnimation("Guard_Break_01", true);
        }
        else {
            character.CharacterAnimatorManager.PlayTargetAnimation(character.CharacterAnimatorManager.BlockHash, true);
        }
    }
}
}