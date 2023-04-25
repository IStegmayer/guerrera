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
        AnimatorManager animatorManager,
        PlayerStats playerStats,
        PlayerCamera playerCamera,
        PlayerWeaponSlotManager playerWeaponSlotManager) {
        base.AttemptToCastSpell(animatorManager, playerStats, playerCamera, playerWeaponSlotManager);
        //TODO: add this camerahandler
        castRotation = playerCamera.GetCameraRotation();
        GameObject warmUpSFXInstance = Instantiate(spellWarmUpFX, playerWeaponSlotManager.GetRightHandTransform());
        animatorManager.PlayTargetAnimation(spellAnimation, true);
    }

    public override void SuccessfullyCastedSpell(AnimatorManager animatorManager, PlayerStats playerStats,
        PlayerCamera playerCamera, PlayerWeaponSlotManager playerWeaponSlotManager) {
        base.SuccessfullyCastedSpell(animatorManager, playerStats, playerCamera, playerWeaponSlotManager);
        animatorManager.Anim.SetBool(animatorManager.IsFiringSpellHash, true);
        GameObject spellFXInstance = Instantiate(spellCastFX, playerWeaponSlotManager.GetRightHandTransform().position,
            castRotation);
        rigidbody = spellFXInstance.GetComponent<Rigidbody>();
        // spellDamageCollider - spellFXInstance.GetComponent<SpellDamageCollider>();

        if (playerCamera.CurrentLockOnTarget != null)
            spellFXInstance.transform.LookAt(playerCamera.CurrentLockOnTarget.transform);
        else
            spellFXInstance.transform.rotation = Quaternion.Euler(playerCamera.GetCameraTransform().eulerAngles.x,
                playerStats.transform.eulerAngles.y, 0);

        rigidbody.AddForce(spellFXInstance.transform.forward * ProjectileForwardVelocity);
        rigidbody.AddForce(spellFXInstance.transform.up * ProjectileUpwardVelocity);
        rigidbody.useGravity = UsesGravity;
        rigidbody.mass = ProjectileMass;
        spellFXInstance.transform.parent = null;
    }
}
}