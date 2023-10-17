using UnityEngine;

namespace Dzajna {
[CreateAssetMenu(menuName = "Item Actions/Heavy Attack Action")]
public class HeavyAttackAction : ItemAction {
    public override void PerformAction(CharacterManager character) {
        if (!character.CharacterStatsManager.CheckHasStamina() || character.IsAttacking) return;

        character.IsAttacking = true;
        // character.characterAnimatorManager.EraseHandIKForWeapon();
        // character.CharacterEffectsManager.PlayRightWeaponFX();

        // Check for running attack
        if (character.IsSprinting) {
            HandleRunningAttack(character);
            return;
        }

        //Check for combo or first attack. If we're interacting, return
        if (character.CanDoCombo) HandleHeavyWeaponCombo(character);
        else if (!character.IsInteracting) HandleHeavyAttack(character);
        else return;

        // update currentAttackType
        character.CharacterCombatManager.CurrentAttackType = AttackType.Heavy;
    }
    
    //TODO: clean these up, too many unnecessary if elses
    private void HandleRunningAttack(CharacterManager character) {
        CharacterAnimatorManager charAnim = character.CharacterAnimatorManager;
        CharacterCombatManager combatManager = character.CharacterCombatManager;

        if (character.IsUsingLeftHand) {
            charAnim.PlayTargetAnimation(charAnim.JumpAttackHash, true, false, true);
            combatManager.LastAttackHash = charAnim.JumpAttackHash;
        }
        else if (character.IsUsingRightHand) {
            if (character.IsTwoHandingWeapon) {
                //TODO: this should be a TH anim
                charAnim.PlayTargetAnimation(charAnim.JumpAttackHash, true);
                combatManager.LastAttackHash = charAnim.JumpAttackHash;
            }
            else {
                charAnim.PlayTargetAnimation(charAnim.JumpAttackHash, true);
                combatManager.LastAttackHash = charAnim.JumpAttackHash;
            }
        }
    }

    private void HandleHeavyAttack(CharacterManager character) {
        CharacterAnimatorManager charAnim = character.CharacterAnimatorManager;

        if (character.IsUsingLeftHand) {
            charAnim.PlayTargetAnimation(charAnim.HeavyAttack1Hash, true, false, true);
            character.CharacterCombatManager.LastAttackHash = charAnim.HeavyAttack1Hash;
        }
        else if (character.IsUsingRightHand) {
            if (character.IsTwoHandingWeapon) {
                charAnim.PlayTargetAnimation(charAnim.TH_HeavyAttack1Hash, true);
                character.CharacterCombatManager.LastAttackHash = charAnim.TH_HeavyAttack1Hash;
            }
            else {
                charAnim.PlayTargetAnimation(charAnim.HeavyAttack1Hash, true);
                character.CharacterCombatManager.LastAttackHash = charAnim.HeavyAttack1Hash;
            }
        }
    }

    private void HandleHeavyWeaponCombo(CharacterManager character) {
        CharacterAnimatorManager charAnim = character.CharacterAnimatorManager;
        CharacterCombatManager combatManager = character.CharacterCombatManager;

        character.animator.SetBool(charAnim.CanDoComboHash, false);

        if (character.IsUsingLeftHand) {
            if (combatManager.LastAttackHash == charAnim.HeavyAttack1Hash) {
                charAnim.PlayTargetAnimation(charAnim.HeavyAttack2Hash, true, false, true);
                combatManager.LastAttackHash = charAnim.HeavyAttack2Hash;
            }
            else {
                charAnim.PlayTargetAnimation(charAnim.HeavyAttack1Hash, true, false, true);
                combatManager.LastAttackHash = charAnim.TH_HeavyAttack1Hash;
            }
        }
        else if (character.IsUsingRightHand) {
            if (character.IsTwoHandingWeapon) {
                if (combatManager.LastAttackHash == charAnim.TH_HeavyAttack1Hash) {
                    charAnim.PlayTargetAnimation(charAnim.TH_HeavyAttack2Hash, true);
                    combatManager.LastAttackHash = charAnim.TH_HeavyAttack2Hash;
                }
                else {
                    charAnim.PlayTargetAnimation(charAnim.TH_HeavyAttack1Hash, true);
                    combatManager.LastAttackHash = charAnim.TH_HeavyAttack1Hash;
                }
            }
            else {
                if (combatManager.LastAttackHash == charAnim.HeavyAttack1Hash) {
                    charAnim.PlayTargetAnimation(charAnim.HeavyAttack2Hash, true);
                    combatManager.LastAttackHash = charAnim.HeavyAttack2Hash;
                }
                else {
                    charAnim.PlayTargetAnimation(charAnim.HeavyAttack1Hash, true);
                    combatManager.LastAttackHash = charAnim.TH_LightAttack1Hash;
                }
            }
        }
    }

    // private void HandleJumpingAttack(CharacterManager character) {
    //     throw new System.NotImplementedException();
    // }
}
}