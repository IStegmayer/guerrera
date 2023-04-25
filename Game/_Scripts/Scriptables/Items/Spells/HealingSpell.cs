using UnityEngine;

namespace Dzajna {
[CreateAssetMenu(menuName = "Spells/Healing Spell")]
public class HealingSpell : SpellItem {
    public int HealAmount;

    public override void AttemptToCastSpell(AnimatorManager animatorManager, PlayerStats playerStats,
        PlayerCamera playerCamera, PlayerWeaponSlotManager playerWeaponSlotManager) {
        base.AttemptToCastSpell(animatorManager, playerStats, playerCamera, playerWeaponSlotManager);
        // GameObject warmUpSpellFXInstance = Instantiate(spellWarmUpFX, animatorManager.transform);
        animatorManager.PlayTargetAnimation(spellAnimation, true);
    }

    public override void SuccessfullyCastedSpell(AnimatorManager animatorManager, PlayerStats playerStats,
        PlayerCamera playerCamera, PlayerWeaponSlotManager playerWeaponSlotManager) {
        base.SuccessfullyCastedSpell(animatorManager, playerStats, playerCamera, playerWeaponSlotManager);
        // GameObject spellFXInstance = Instantiate(spellCastFX, animatorManager.transform);
        // animatorManager.PlayTargetAnimation(spellAnimation, true);
        playerStats.TakeDamage(-HealAmount);
    }
}
}