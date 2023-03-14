using UnityEngine;

namespace Dzajna {
[CreateAssetMenu(menuName = "Spells/Healing Spell")]
public class HealingSpell : SpellItem {
    public int HealAmount;

    public override void AttemptToCastSpell(AnimatorManager _animatorManager, PlayerStats _playerStats,
        CameraManager _cameraManager, WeaponSlotManager _weaponSlotManager) {
        base.AttemptToCastSpell(_animatorManager, _playerStats, _cameraManager, _weaponSlotManager);
        // GameObject _warmUpSpellFXInstance = Instantiate(spellWarmUpFX, _animatorManager.transform);
        _animatorManager.PlayTargetAnimation(spellAnimation, true);
    }

    public override void SuccessfullyCastedSpell(AnimatorManager _animatorManager, PlayerStats _playerStats,
        CameraManager _cameraManager, WeaponSlotManager _weaponSlotManager) {
        base.SuccessfullyCastedSpell(_animatorManager, _playerStats, _cameraManager, _weaponSlotManager);
        // GameObject _spellFXInstance = Instantiate(spellCastFX, _animatorManager.transform);
        // _animatorManager.PlayTargetAnimation(spellAnimation, true);
        _playerStats.TakeDamage(-HealAmount);
    }
}
}