using UnityEngine;

namespace Dzajna {
[CreateAssetMenu(menuName = "Spells/Healing Spell")]
public class HealingSpell : SpellItem {
    public int HealAmount;

    public override void AttemptToCastSpell(AnimatorManager _animatorManager, PlayerStats _playerStats) {
        base.AttemptToCastSpell(_animatorManager, _playerStats);
        // GameObject _warmUpSpellFXInstance = Instantiate(spellWarmUpFX, _animatorManager.transform);
        _animatorManager.PlayTargetAnimation(spellAnimation, true);
    }

    public override void SuccessfullyCastedSpell(AnimatorManager _animatorManager, PlayerStats _playerStats) {
        base.SuccessfullyCastedSpell(_animatorManager, _playerStats);
        // GameObject _spellFXInstance = Instantiate(spellCastFX, _animatorManager.transform);
        // _animatorManager.PlayTargetAnimation(spellAnimation, true);
        _playerStats.TakeDamage(-HealAmount);
    }
}
}