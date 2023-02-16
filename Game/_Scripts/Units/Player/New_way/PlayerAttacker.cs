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

    private void Awake() {
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        playerManager = GetComponent<PlayerManager>();
        playerInventory = GetComponent<PlayerInventory>();
        playerStats = GetComponent<PlayerStats>();
        inputHandler = GetComponent<InputHandler>();
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
    }

    public void HandleRBAction() {
        WeaponItem _weapon = playerInventory.RightWeapon;
        if (_weapon.isMeleeWeapon) PerformRBMeleeAction(_weapon);
        else if (_weapon.isMagicCaster) {
            PerformRBMagicCast(_weapon);
        }
        else if (_weapon.isFaithCaster) {
            PerformRBFaithCast(_weapon);
        }
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
        SpellItem _spell = playerInventory.CurrentSpell;
        if (_spell != null && _spell.isMagicSpell) {
            // Check for FP
            _spell.AttemptToCastSpell(playerAnimatorManager, playerStats);
        }
    }

    private void PerformRBFaithCast(WeaponItem _weapon) {
        SpellItem _spell = playerInventory.CurrentSpell;
        if (_spell != null && _spell.isFaithSpell) {
            // Check for FP
            _spell.AttemptToCastSpell(playerAnimatorManager, playerStats);
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

    public void HandleHeavyAttack(WeaponItem _weaponItem) {
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