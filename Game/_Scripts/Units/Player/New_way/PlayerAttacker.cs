using System;
using UnityEngine;

namespace Dzajna {
// TODO: All this hash management is very inefficient, weaponItem should have its animations already hashed 
// also combo can just be an array of attacks and have a tree in anim. Overall a faulty script
public class PlayerAttacker : MonoBehaviour {
    private PlayerAnimatorManager playerAnimatorManager;
    private InputHandler inputHandler;
    private WeaponSlotManager weaponSlotManager;
    public string LastAttackString;

    private void Awake() {
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        inputHandler = GetComponent<InputHandler>();
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
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

    public void HandleLightAttack(WeaponItem _weaponItem) {
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