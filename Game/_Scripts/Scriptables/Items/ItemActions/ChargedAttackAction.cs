using UnityEngine;

namespace Dzajna {
[CreateAssetMenu(menuName = "Item Actions/Charged Attack Action")]
public class ChargedAttackAction : ItemAction {
    public override void PerformAction(CharacterManager character) {
        if (!character.CharacterStatsManager.CheckHasStamina() || character.IsAttacking) return;
        if (character.IsInteracting) return;

        // character.CharacterAnimatorManager.EraseHandIKForWeapon();
        // possibly combo here

        character.IsAttacking = true;
        character.CharacterCombatManager.CurrentAttackType = AttackType.Heavy;

        HandleChargeAttack(character);
    }

    private void HandleChargeAttack(CharacterManager character) {
        CharacterAnimatorManager charAnim = character.CharacterAnimatorManager;
        CharacterCombatManager combatManager = character.CharacterCombatManager;

        if (character.IsUsingLeftHand) {
            charAnim.PlayTargetAnimation(charAnim.ChargedAttackHash, true, false, true);
            combatManager.LastAttackHash = charAnim.ChargedAttackHash;
        }
        else if (character.IsUsingRightHand) {
            if (character.IsTwoHandingWeapon) {
                charAnim.PlayTargetAnimation(charAnim.ChargedAttackHash, true);
                combatManager.LastAttackHash = charAnim.ChargedAttackHash;
            }
            else {
                charAnim.PlayTargetAnimation(charAnim.ChargedAttackHash, true);
                combatManager.LastAttackHash = charAnim.ChargedAttackHash;
            }
        }
    }
}
}