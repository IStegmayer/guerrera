using System;
using UnityEngine;

namespace Dzajna {
// TODO: All this hash management is very inefficient, weaponItem should have its animations already hashed 
// also combo can just be an array of attacks and have a tree in anim. Overall a faulty script
public class PlayerCombatManager : MonoBehaviour {
    private PlayerAnimatorManager playerAnimatorManager;
    private PlayerEquipmentManager playerEquipmentManager;
    private PlayerManager playerManager;
    private PlayerInventoryManager playerInventoryManager;
    private PlayerStats playerStats;
    private PlayerWeaponSlotManager playerWeaponSlotManager;
    private PlayerEffectsManager playerEffectsManager;
    private InputHandler inputHandler;
    private CameraManager cameraManager;
    public string LastAttackString;

    private float maxCritDistance = 0.75f;
    [SerializeField] private Transform critRayCastStartPoint;
    [SerializeField] private LayerMask backStabLayer;
    [SerializeField] private LayerMask riposteLayer;

    private void Awake() {
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        playerManager = GetComponent<PlayerManager>();
        playerInventoryManager = GetComponent<PlayerInventoryManager>();
        playerStats = GetComponent<PlayerStats>();
        inputHandler = InputHandler.Instance;
        playerEffectsManager = GetComponent<PlayerEffectsManager>();
        playerWeaponSlotManager = GetComponentInChildren<PlayerWeaponSlotManager>();
        cameraManager = CameraManager.Instance;
    }

    public void HandleLTAction() {
        WeaponItem weapon = playerInventoryManager.LeftWeapon;
        if (weapon.isShieldWeapon) {
            PerformLTWeaponArt(weapon, false);
        }
    }

    public void HandleRBAction() {
        if (!playerStats.CheckHasStamina()) return;
        WeaponItem weapon = playerInventoryManager.RightWeapon;
        if (weapon.isMeleeWeapon) PerformRBMeleeAction(weapon);
        else if (weapon.isMagicCaster) {
            PerformRBMagicCast(weapon);
        }
        else if (weapon.isFaithCaster) {
            PerformRBFaithCast(weapon);
        }
    }

    public void HandleLBAction() {
        PerformLBBlockingAction();
    }

    private void PerformRBMeleeAction(WeaponItem weapon) {
        if (playerManager.CanDoCombo) {
            inputHandler.SetComboFlag(true);
            HandleWeaponCombo(weapon);
            inputHandler.SetComboFlag(false);
        }
        else if (!playerManager.IsInteracting && !playerManager.CanDoCombo) {
            playerAnimatorManager.Anim.SetBool(playerAnimatorManager.IsUsingRightHandHash, true);
            HandleLightAttack(weapon);
        }
    }

    private void PerformRBMagicCast(WeaponItem weapon) {
        if (playerManager.IsInteracting) return;

        SpellItem spell = playerInventoryManager.CurrentSpell;
        if (spell != null && spell.isMagicSpell) {
            bool canCast = playerStats.CheckIfCanCast(spell.FPCost);
            if (canCast) spell.AttemptToCastSpell(playerAnimatorManager, playerStats, cameraManager, playerWeaponSlotManager);
            else playerAnimatorManager.PlayTargetAnimation(playerAnimatorManager.FailedCastHash, true);
        }
    }

    private void PerformRBFaithCast(WeaponItem weapon) {
        if (playerManager.IsInteracting) return;

        SpellItem spell = playerInventoryManager.CurrentSpell;
        if (spell != null && spell.isFaithSpell) {
            bool canCast = playerStats.CheckIfCanCast(spell.FPCost);
            if (canCast) spell.AttemptToCastSpell(playerAnimatorManager, playerStats, cameraManager, playerWeaponSlotManager);
            else playerAnimatorManager.PlayTargetAnimation(playerAnimatorManager.FailedCastHash, true);
        }
    }

    private void PerformLTWeaponArt(WeaponItem weapon, bool isTwoHanding) {
        if (playerManager.IsInteracting) return;

        if (isTwoHanding) {
        }
        else {
            playerAnimatorManager.PlayTargetAnimation(weapon.WeaponArt, true);
        }
        
        // If we are two handing perform weapon art for right weapon
        // else perform art for left handed weapon
        
    }

    public void AttemptBackStabOrRiposte() {
        if (!playerStats.CheckHasStamina()) return;

        RaycastHit hit;

        if (Physics.Raycast(critRayCastStartPoint.position, transform.TransformDirection(Vector3.forward), out hit,
                maxCritDistance, backStabLayer)) {
            CharacterManager enemyCharacterManager = hit.transform.GetComponent<CharacterManager>();
            DamageCollider rightWeapon = playerWeaponSlotManager.RightDamageCollider;

            if (enemyCharacterManager == null) return;
            // Pull us in position to backstab
            playerManager.transform.position = enemyCharacterManager.BackStabCollider.GetAttackerStandPoint().position;
            // rotate towards transform
            Vector3 rotationDirection = playerManager.transform.root.eulerAngles;
            rotationDirection = hit.transform.position - playerManager.transform.position;
            rotationDirection.y = 0;
            rotationDirection.Normalize();
            Quaternion tr = Quaternion.LookRotation(rotationDirection);
            Quaternion targetRotation =
                Quaternion.Slerp(playerManager.transform.rotation, tr, 500f * Time.deltaTime);
            playerManager.transform.rotation = targetRotation;

            // play animation
            playerAnimatorManager.PlayTargetAnimation(playerAnimatorManager.BackStabHash, true);
            // make enemy play animation
            enemyCharacterManager.GetComponent<EnemyAnimatorManager>()
                .PlayTargetAnimation(playerAnimatorManager.BackStabbedHash, true);

            // deal dmg
            // TODO: this is not taking into account the player stats
            float critDamage = playerInventoryManager.RightWeapon.BaseDamage *
                                playerInventoryManager.RightWeapon.CriticalDamageMultiplier;
            enemyCharacterManager.PendingCriticalDamage = critDamage;
        } else if (Physics.Raycast(critRayCastStartPoint.position, transform.TransformDirection(Vector3.forward),
                       out hit, maxCritDistance, riposteLayer)) {
            //TODO: repeating code
            CharacterManager enemyCharacterManager = hit.transform.GetComponent<CharacterManager>();
            DamageCollider rightWeapon = playerWeaponSlotManager.RightDamageCollider;

            if (enemyCharacterManager == null || !enemyCharacterManager.CanBeRiposted) return;
            
            playerManager.transform.position = enemyCharacterManager.RiposteCollider.GetAttackerStandPoint().position;
            // rotate towards transform
            Vector3 rotationDirection = playerManager.transform.root.eulerAngles;
            rotationDirection = hit.transform.position - playerManager.transform.position;
            rotationDirection.y = 0;
            rotationDirection.Normalize();
            Quaternion tr = Quaternion.LookRotation(rotationDirection);
            Quaternion targetRotation =
                Quaternion.Slerp(playerManager.transform.rotation, tr, 500f * Time.deltaTime);
            playerManager.transform.rotation = targetRotation;
            
            // play animation
            playerAnimatorManager.PlayTargetAnimation(playerAnimatorManager.RiposteHash, true);
            // make enemy play animation
            EnemyAnimatorManager anm = enemyCharacterManager.GetComponent<EnemyAnimatorManager>();
            anm.PlayTargetAnimation(playerAnimatorManager.RipostedHash, true);

            // deal dmg
            // TODO: this is not taking into account the player stats
            float critDamage = playerInventoryManager.RightWeapon.BaseDamage *
                                playerInventoryManager.RightWeapon.CriticalDamageMultiplier;
            enemyCharacterManager.PendingCriticalDamage = critDamage;
        }
    }

    public void SuccessfullyCastSpell() {
        playerInventoryManager.CurrentSpell.SuccessfullyCastedSpell(playerAnimatorManager, playerStats, cameraManager, playerWeaponSlotManager);
    }

    public void HandleWeaponCombo(WeaponItem weaponItem) {
        if (!inputHandler.ComboFlag) return;

        //TODO: not good
        if (LastAttackString == weaponItem.OH_Light_Attack_1) {
            playerAnimatorManager.PlayTargetAnimation(Animator.StringToHash(weaponItem.OH_Light_Attack_2), true);
        }
        else if (LastAttackString == weaponItem.OH_Heavy_Attack_1) {
            playerAnimatorManager.PlayTargetAnimation(Animator.StringToHash(weaponItem.OH_Heavy_Attack_2), true);
        }
        else if (LastAttackString == weaponItem.TH_Light_Attack_1) {
            playerAnimatorManager.PlayTargetAnimation(Animator.StringToHash(weaponItem.TH_Light_Attack_2), true);
        }
        else if (LastAttackString == weaponItem.TH_Heavy_Attack_1) {
            playerAnimatorManager.PlayTargetAnimation(Animator.StringToHash(weaponItem.TH_Heavy_Attack_2), true);
        }

        playerAnimatorManager.Anim.SetBool(Animator.StringToHash("canDoCombo"), false);
    }

    private void HandleLightAttack(WeaponItem weaponItem) {
        playerWeaponSlotManager.SetAttackingWeapon(weaponItem);

        if (inputHandler.TwoHandFlag) {
            playerAnimatorManager.PlayTargetAnimation(weaponItem.TH_Light_Attack_1, true);
            LastAttackString = weaponItem.TH_Light_Attack_1;
        }
        else {
            playerAnimatorManager.PlayTargetAnimation(weaponItem.OH_Light_Attack_1, true);
            LastAttackString = weaponItem.OH_Light_Attack_1;
        }
    }

    private void PerformLBBlockingAction() {
        if (playerManager.IsInteracting) return;
        if (playerManager.IsBlocking) return;
        
        playerAnimatorManager.PlayTargetAnimation(playerAnimatorManager.BlockHash, false, true);
        playerManager.IsBlocking = true;
        playerEquipmentManager.OpenBlockingCollider();
    }

    // TODO: im checking here for now
    public void HandleHeavyAttack(WeaponItem weaponItem) {
        if (!playerStats.CheckHasStamina()) return;
        
        playerWeaponSlotManager.SetAttackingWeapon(weaponItem);
        if (inputHandler.TwoHandFlag) {
            playerAnimatorManager.PlayTargetAnimation(weaponItem.TH_Heavy_Attack_1, true);
            LastAttackString = weaponItem.TH_Heavy_Attack_1;
        }
        else {
            playerAnimatorManager.PlayTargetAnimation(weaponItem.OH_Heavy_Attack_1, true);
            LastAttackString = weaponItem.OH_Heavy_Attack_1;
        }
    }
}
}