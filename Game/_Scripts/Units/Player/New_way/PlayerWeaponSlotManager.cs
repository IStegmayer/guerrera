using System;
using UnityEngine;

namespace Dzajna {
public class PlayerWeaponSlotManager : CharacterWeaponSlotManager {
    private WeaponItem attackingWeapon;

    private PlayerManager playerManager;
    private Animator animator;
    private QuickSlotsUI quickSlotsUI;
    private PlayerStats playerStats;
    private InputHandler inputHandler;


    private void Awake() {
        animator = GetComponent<Animator>();
        playerStats = GetComponentInParent<PlayerStats>();
        inputHandler = GetComponentInParent<InputHandler>();
        playerManager = GetComponentInParent<PlayerManager>();
        //TODO: im still using this...
        quickSlotsUI = FindObjectOfType<QuickSlotsUI>();
        
        LoadWeaponHolderSlots();
    }

    private void LoadWeaponHolderSlots() {
        WeaponHolderSlot[] _weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
        foreach (WeaponHolderSlot _weaponSlot in _weaponHolderSlots) {
            if (_weaponSlot.isLeftHandSlot) leftHandSlot = _weaponSlot;
            else if (_weaponSlot.isRightHandSlot) rightHandSlot = _weaponSlot;
            else if (_weaponSlot.isBackSlot) backSlot = _weaponSlot;
        }
    }

    public void LoadWeaponOnSlot(WeaponItem _weaponItem, bool _isLeft) {
        WeaponItem _weaponToEquip = _weaponItem != null ? _weaponItem : unarmedWeapon;
        
        if (_isLeft) {
            leftHandSlot.CurrentWeaponItem = _weaponToEquip;
            leftHandSlot.LoadWeaponModel(_weaponToEquip);
            LoadLeftWeaponDamageCollider();
            quickSlotsUI.UpdateWeaponQuickSlotsUI(_weaponToEquip, true);
            animator.CrossFade(_weaponToEquip.OH_LeftArmIdle, 0.2f);
        }
        else {
            if (inputHandler.TwoHandFlag) {
                backSlot.LoadWeaponModel(leftHandSlot.CurrentWeaponItem);
                animator.CrossFade(_weaponToEquip.TH_Idle, 0.2f);
                leftHandSlot.UnloadWeaponAndDestroy();
            }
            else {
                animator.CrossFade(_weaponToEquip.OH_RightArmIdle, 0.2f);
                backSlot.UnloadWeaponAndDestroy();
            }

            rightHandSlot.CurrentWeaponItem = _weaponToEquip;
            rightHandSlot.LoadWeaponModel(_weaponToEquip);
            LoadRightWeaponDamageCollider();
            quickSlotsUI.UpdateWeaponQuickSlotsUI(_weaponToEquip, false);
        }
    }

    #region Handle Weapon's Damage Collider

    private void LoadLeftWeaponDamageCollider() {
        LeftDamageCollider =
            leftHandSlot.CurrentWeaponModel.GetComponentInChildren<DamageCollider>();
        LeftDamageCollider.CurrentWeaponDamage = leftHandSlot.CurrentWeaponItem.BaseDamage;
        LeftDamageCollider.PoiseBreak = leftHandSlot.CurrentWeaponItem.PoiseBreak;
        LeftDamageCollider.OffensivePoiseBonus = leftHandSlot.CurrentWeaponItem.OffensivePoiseBonus;
    }

    private void LoadRightWeaponDamageCollider() {
        RightDamageCollider =
            rightHandSlot.CurrentWeaponModel.GetComponentInChildren<DamageCollider>();
        RightDamageCollider.CurrentWeaponDamage = rightHandSlot.CurrentWeaponItem.BaseDamage;
        RightDamageCollider.PoiseBreak = rightHandSlot.CurrentWeaponItem.PoiseBreak;
        RightDamageCollider.OffensivePoiseBonus = rightHandSlot.CurrentWeaponItem.OffensivePoiseBonus;
    } 

    public void OpenDamageCollider() {
        if (playerManager.IsUsingLeftHand)
            LeftDamageCollider.EnableDamageCollider();
        else if (playerManager.IsUsingRightHand)
            RightDamageCollider.EnableDamageCollider();
    }
    public void CloseDamageCollider() {
        if (LeftDamageCollider != null) LeftDamageCollider.DisableDamageCollider();
        if (RightDamageCollider != null) RightDamageCollider.DisableDamageCollider();
    }

    #endregion

    public void SetAttackingWeapon(WeaponItem _attackingWeaponItem) {
        attackingWeapon = _attackingWeaponItem;
    }

    //TODO: again this could be compressed...
    public void DrainStaminaLightAttack() {
        playerStats.TakeStaminaDamage(
            Mathf.RoundToInt(attackingWeapon.BaseStamina * attackingWeapon.LightAttackMultiplier));
    }
    public void DrainStaminaHeavyAttack() {
        playerStats.TakeStaminaDamage(
            Mathf.RoundToInt(attackingWeapon.BaseStamina * attackingWeapon.HeavyAttackMultiplier));
    }
    
    public void GrantWeaponAttackingPoiseBonus() {
        playerStats.TotalPoise += attackingWeapon.OffensivePoiseBonus;
    }
    public void RemoveWeaponAttackingPoiseBonus() {
        playerStats.TotalPoise -= attackingWeapon.OffensivePoiseBonus;
    }
    

    public Transform GetRightHandTransform() => rightHandSlot.transform;
}
}