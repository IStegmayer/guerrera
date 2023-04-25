using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Dzajna {
public class PlayerWeaponSlotManager : CharacterWeaponSlotManager {
    private WeaponItem attackingWeapon;

    private PlayerManager playerManager;
    private Animator animator;
    private QuickSlotsUI quickSlotsUI;
    private PlayerStats playerStats;
    private PlayerEffectsManager playerEffectsManager;

    private void Awake() {
        animator = GetComponent<Animator>();
        playerStats = GetComponentInParent<PlayerStats>();
        playerManager = GetComponentInParent<PlayerManager>();
        playerEffectsManager = GetComponentInParent<PlayerEffectsManager>();
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
            if (InputHandler.Instance.TwoHandFlag) {
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
        WeaponItem _weapon = leftHandSlot.CurrentWeaponItem;
        GameObject _weaponModel = leftHandSlot.CurrentWeaponModel;
        
        LeftDamageCollider = _weaponModel.GetComponentInChildren<DamageCollider>();
        LeftDamageCollider.CurrentWeaponDamage = _weapon.BaseDamage;
        LeftDamageCollider.PoiseBreak = _weapon.PoiseBreak;
        LeftDamageCollider.OffensivePoiseBonus = _weapon.OffensivePoiseBonus;

        playerEffectsManager.LeftWeaponFX = _weaponModel.GetComponentInChildren<WeaponFX>();
    }

    private void LoadRightWeaponDamageCollider() {
        WeaponItem _weapon = rightHandSlot.CurrentWeaponItem;
        GameObject _weaponModel = rightHandSlot.CurrentWeaponModel;
        
        RightDamageCollider = _weaponModel.GetComponentInChildren<DamageCollider>();
        RightDamageCollider.CurrentWeaponDamage = _weapon.BaseDamage;
        RightDamageCollider.PoiseBreak = _weapon.PoiseBreak;
        RightDamageCollider.OffensivePoiseBonus = _weapon.OffensivePoiseBonus;
        
        playerEffectsManager.RightWeaponFX = _weaponModel.GetComponentInChildren<WeaponFX>();
    } 

    public void OpenDamageCollider() {
        if (playerManager.IsUsingLeftHand) LeftDamageCollider.EnableDamageCollider();
        else if (playerManager.IsUsingRightHand) RightDamageCollider.EnableDamageCollider();
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