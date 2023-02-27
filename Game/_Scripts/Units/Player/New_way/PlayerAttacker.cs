using System;
using UnityEngine;

namespace Dzajna {
// TODO: All this hash management is very inefficient, weaponItem should have its animations already hashed 
// also combo can just be an array of attacks and have a tree in anim. Overall a faulty script
public class PlayerAttacker : MonoBehaviour {
    private PlayerAnimatorManager playerAnimatorManager;
    private PlayerManager playerManager;
    private PlayerInventory playerInventory;
    private PlayerStats playerStats;
    private InputHandler inputHandler;
    private WeaponSlotManager weaponSlotManager;
    public string LastAttackString;

    private float maxCritDistance = 0.75f;
    [SerializeField] private Transform critRayCastStartPoint;
    [SerializeField] private LayerMask backStabLayer;
    [SerializeField] private LayerMask riposteLayer;

    private void Awake() {
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        playerManager = GetComponent<PlayerManager>();
        playerInventory = GetComponent<PlayerInventory>();
        playerStats = GetComponent<PlayerStats>();
        inputHandler = GetComponent<InputHandler>();
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
    }

    public void HandleLTAction() {
        WeaponItem _weapon = playerInventory.LeftWeapon;
        if (_weapon.isShieldWeapon) {
            PerformLTWeaponArt(_weapon, false);
        }
    }

    public void HandleRBAction() {
        if (!playerStats.CheckHasStamina()) return;
        WeaponItem _weapon = playerInventory.RightWeapon;
        if (_weapon.isMeleeWeapon) PerformRBMeleeAction(_weapon);
        else if (_weapon.isMagicCaster) {
            PerformRBMagicCast(_weapon);
        }
        else if (_weapon.isFaithCaster) {
            PerformRBFaithCast(_weapon);
        }
    }

    public void HandleLBAction() {
        PerformLBBlockingAction();
    }

    private void PerformRBMeleeAction(WeaponItem _weapon) {
        if (playerManager.CanDoCombo) {
            inputHandler.ComboFlag = true;
            HandleWeaponCombo(_weapon);
            inputHandler.ComboFlag = false;
        }
        else if (!playerManager.IsInteracting && !playerManager.CanDoCombo) {
            playerAnimatorManager.Anim.SetBool(playerAnimatorManager.IsUsingRightHandHash, true);
            HandleLightAttack(_weapon);
        }
    }

    private void PerformRBMagicCast(WeaponItem _weapon) {
        if (playerManager.IsInteracting) return;

        SpellItem _spell = playerInventory.CurrentSpell;
        if (_spell != null && _spell.isMagicSpell) {
            bool _canCast = playerStats.CheckIfCanCast(_spell.FPCost);
            if (_canCast) _spell.AttemptToCastSpell(playerAnimatorManager, playerStats);
            else playerAnimatorManager.PlayTargetAnimation(playerAnimatorManager.FailedCastHash, true);
        }
    }

    private void PerformRBFaithCast(WeaponItem _weapon) {
        if (playerManager.IsInteracting) return;

        SpellItem _spell = playerInventory.CurrentSpell;
        if (_spell != null && _spell.isFaithSpell) {
            bool _canCast = playerStats.CheckIfCanCast(_spell.FPCost);
            if (_canCast) _spell.AttemptToCastSpell(playerAnimatorManager, playerStats);
            else playerAnimatorManager.PlayTargetAnimation(playerAnimatorManager.FailedCastHash, true);
        }
    }

    private void PerformLTWeaponArt(WeaponItem _weapon, bool _isTwoHanding) {
        if (playerManager.IsInteracting) return;

        if (_isTwoHanding) {
        }
        else {
            playerAnimatorManager.PlayTargetAnimation(_weapon.WeaponArt, true);
        }
        
        // If we are two handing perform weapon art for right weapon
        // else perform art for left handed weapon
        
    }

    public void AttemptBackStabOrRiposte() {
        if (!playerStats.CheckHasStamina()) return;

        RaycastHit hit;

        if (Physics.Raycast(critRayCastStartPoint.position, transform.TransformDirection(Vector3.forward), out hit,
                maxCritDistance, backStabLayer)) {
            CharacterManager _enemyCharacterManager = hit.transform.GetComponent<CharacterManager>();
            DamageCollider _rightWeapon = weaponSlotManager.RightDamageCollider;

            if (_enemyCharacterManager == null) return;
            // Pull us in position to backstab
            playerManager.transform.position = _enemyCharacterManager.BackStabCollider.GetAttackerStandPoint().position;
            // rotate towards transform
            Vector3 _rotationDirection = playerManager.transform.root.eulerAngles;
            _rotationDirection = hit.transform.position - playerManager.transform.position;
            _rotationDirection.y = 0;
            _rotationDirection.Normalize();
            Quaternion _tr = Quaternion.LookRotation(_rotationDirection);
            Quaternion _targetRotation =
                Quaternion.Slerp(playerManager.transform.rotation, _tr, 500f * Time.deltaTime);
            playerManager.transform.rotation = _targetRotation;

            // play animation
            playerAnimatorManager.PlayTargetAnimation(playerAnimatorManager.BackStabHash, true);
            // make enemy play animation
            _enemyCharacterManager.GetComponent<EnemyAnimatorManager>()
                .PlayTargetAnimation(playerAnimatorManager.BackStabbedHash, true);

            // deal dmg
            // TODO: this is not taking into account the player stats
            float _critDamage = playerInventory.RightWeapon.BaseDamage *
                                playerInventory.RightWeapon.CriticalDamageMultiplier;
            _enemyCharacterManager.PendingCriticalDamage = _critDamage;
        } else if (Physics.Raycast(critRayCastStartPoint.position, transform.TransformDirection(Vector3.forward),
                       out hit, maxCritDistance, riposteLayer)) {
            //TODO: repeating code
            CharacterManager _enemyCharacterManager = hit.transform.GetComponent<CharacterManager>();
            DamageCollider _rightWeapon = weaponSlotManager.RightDamageCollider;

            if (_enemyCharacterManager == null || !_enemyCharacterManager.CanBeRiposted) return;
            
            playerManager.transform.position = _enemyCharacterManager.RiposteCollider.GetAttackerStandPoint().position;
            // rotate towards transform
            Vector3 _rotationDirection = playerManager.transform.root.eulerAngles;
            _rotationDirection = hit.transform.position - playerManager.transform.position;
            _rotationDirection.y = 0;
            _rotationDirection.Normalize();
            Quaternion _tr = Quaternion.LookRotation(_rotationDirection);
            Quaternion _targetRotation =
                Quaternion.Slerp(playerManager.transform.rotation, _tr, 500f * Time.deltaTime);
            playerManager.transform.rotation = _targetRotation;
            
            // play animation
            playerAnimatorManager.PlayTargetAnimation(playerAnimatorManager.RiposteHash, true);
            // make enemy play animation
            EnemyAnimatorManager anm = _enemyCharacterManager.GetComponent<EnemyAnimatorManager>();
            anm.PlayTargetAnimation(playerAnimatorManager.RipostedHash, true);

            // deal dmg
            // TODO: this is not taking into account the player stats
            float _critDamage = playerInventory.RightWeapon.BaseDamage *
                                playerInventory.RightWeapon.CriticalDamageMultiplier;
            _enemyCharacterManager.PendingCriticalDamage = _critDamage;
        }
    }

    public void SuccessfullyCastSpell() {
        playerInventory.CurrentSpell.SuccessfullyCastedSpell(playerAnimatorManager, playerStats);
    }

    public void HandleWeaponCombo(WeaponItem _weaponItem) {
        if (!inputHandler.ComboFlag) return;

        //TODO: not good
        if (LastAttackString == _weaponItem.OH_Light_Attack_1) {
            playerAnimatorManager.PlayTargetAnimation(Animator.StringToHash(_weaponItem.OH_Light_Attack_2), true);
        }
        else if (LastAttackString == _weaponItem.OH_Heavy_Attack_1) {
            playerAnimatorManager.PlayTargetAnimation(Animator.StringToHash(_weaponItem.OH_Heavy_Attack_2), true);
        }
        else if (LastAttackString == _weaponItem.TH_Light_Attack_1) {
            playerAnimatorManager.PlayTargetAnimation(Animator.StringToHash(_weaponItem.TH_Light_Attack_2), true);
        }
        else if (LastAttackString == _weaponItem.TH_Heavy_Attack_1) {
            playerAnimatorManager.PlayTargetAnimation(Animator.StringToHash(_weaponItem.TH_Heavy_Attack_2), true);
        }

        playerAnimatorManager.Anim.SetBool(Animator.StringToHash("canDoCombo"), false);
    }

    private void HandleLightAttack(WeaponItem _weaponItem) {
        weaponSlotManager.SetAttackingWeapon(_weaponItem);

        if (inputHandler.TwoHandFlag) {
            playerAnimatorManager.PlayTargetAnimation(_weaponItem.TH_Light_Attack_1, true);
            LastAttackString = _weaponItem.TH_Light_Attack_1;
        }
        else {
            playerAnimatorManager.PlayTargetAnimation(_weaponItem.OH_Light_Attack_1, true);
            LastAttackString = _weaponItem.OH_Light_Attack_1;
        }
    }

    private void PerformLBBlockingAction() {
        if (playerManager.IsInteracting) return;
        if (playerManager.IsBlocking) return;
        
        playerAnimatorManager.PlayTargetAnimation(playerAnimatorManager.BlockHash, false, true);
        playerManager.IsBlocking = true;
    }

    // TODO: im checking here for now
    public void HandleHeavyAttack(WeaponItem _weaponItem) {
        if (!playerStats.CheckHasStamina()) return;
        
        weaponSlotManager.SetAttackingWeapon(_weaponItem);
        if (inputHandler.TwoHandFlag) {
            playerAnimatorManager.PlayTargetAnimation(_weaponItem.TH_Heavy_Attack_1, true);
            LastAttackString = _weaponItem.TH_Heavy_Attack_1;
        }
        else {
            playerAnimatorManager.PlayTargetAnimation(_weaponItem.OH_Heavy_Attack_1, true);
            LastAttackString = _weaponItem.OH_Heavy_Attack_1;
        }
    }
}
}