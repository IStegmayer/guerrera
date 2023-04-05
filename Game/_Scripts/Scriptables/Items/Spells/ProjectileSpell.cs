using UnityEngine;

namespace Dzajna {
[CreateAssetMenu(menuName = "Spells/ProjectileSpell")]
public class ProjectileSpell : SpellItem {
    public float BaseDamage;

    [Header("Projectile physics")] public float ProjectileForwardVelocity;
    public float ProjectileUpwardVelocity;
    public float ProjectileMass;
    public bool UsesGravity;

    private Rigidbody rigidbody;
    private Quaternion castRotation;

    public override void AttemptToCastSpell(
        AnimatorManager _animatorManager,
        PlayerStats _playerStats,
        CameraManager _cameraManager,
        PlayerWeaponSlotManager _playerWeaponSlotManager) {
        base.AttemptToCastSpell(_animatorManager, _playerStats, _cameraManager, _playerWeaponSlotManager);
        //TODO: add this camerahandler
        castRotation = _cameraManager.GetCameraRotation();
        GameObject _warmUpSFXInstance = Instantiate(spellWarmUpFX, _playerWeaponSlotManager.GetRightHandTransform());
        _animatorManager.PlayTargetAnimation(spellAnimation, true);
    }

    public override void SuccessfullyCastedSpell(AnimatorManager _animatorManager, PlayerStats _playerStats,
        CameraManager _cameraManager, PlayerWeaponSlotManager _playerWeaponSlotManager) {
        base.SuccessfullyCastedSpell(_animatorManager, _playerStats, _cameraManager, _playerWeaponSlotManager);
        _animatorManager.Anim.SetBool(_animatorManager.IsFiringSpellHash, true);
        GameObject _spellFXInstance = Instantiate(spellCastFX, _playerWeaponSlotManager.GetRightHandTransform().position,
            castRotation);
        rigidbody = _spellFXInstance.GetComponent<Rigidbody>();
        // spellDamageCollider - _spellFXInstance.GetComponent<SpellDamageCollider>();

        if (_cameraManager.CurrentLockOnTarget != null)
            _spellFXInstance.transform.LookAt(_cameraManager.CurrentLockOnTarget.transform);
        else
            _spellFXInstance.transform.rotation = Quaternion.Euler(_cameraManager.GetCameraTransform().eulerAngles.x,
                _playerStats.transform.eulerAngles.y, 0);

        rigidbody.AddForce(_spellFXInstance.transform.forward * ProjectileForwardVelocity);
        rigidbody.AddForce(_spellFXInstance.transform.up * ProjectileUpwardVelocity);
        rigidbody.useGravity = UsesGravity;
        rigidbody.mass = ProjectileMass;
        _spellFXInstance.transform.parent = null;
    }
}
}