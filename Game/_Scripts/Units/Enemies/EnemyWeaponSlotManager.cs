using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dzajna {
public class EnemyWeaponSlotManager : CharacterWeaponSlotManager {

    private EnemyStatsManager enemyStatsManager;

    private void Awake() {
        enemyStatsManager = GetComponentInParent<EnemyStatsManager>();
        LoadWeaponHolderSlots();
    }

    private void Start() {
        if (rightHandWeapon != null) LoadWeaponOnSlot(rightHandWeapon, false);
        if (leftHandWeapon != null) LoadWeaponOnSlot(leftHandWeapon, true);
    }

    private void LoadWeaponHolderSlots() {
        WeaponHolderSlot[] _weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
        foreach (WeaponHolderSlot _weaponSlot in _weaponHolderSlots) {
            if (_weaponSlot.isLeftHandSlot) leftHandSlot = _weaponSlot;
            else if (_weaponSlot.isRightHandSlot) rightHandSlot = _weaponSlot;
        }
    }

    private void LoadWeaponOnSlot(WeaponItem _weapon, bool _isLeft) {
        WeaponItem _weaponToEquip = _weapon != null ? _weapon : unarmedWeapon;

        if (_isLeft) {
            leftHandSlot.CurrentWeaponItem = _weaponToEquip;
            leftHandSlot.LoadWeaponModel(_weaponToEquip);
            LoadLeftWeaponDamageCollider();
        }
        else {
            rightHandSlot.CurrentWeaponItem = _weaponToEquip;
            rightHandSlot.LoadWeaponModel(_weaponToEquip);
            LoadRightWeaponDamageCollider();
        }
    }

    private void LoadWeaponsDamageCollider(bool _isLeft) {
        if (_isLeft) {
            LeftDamageCollider = leftHandSlot.CurrentWeaponModel.GetComponentInChildren<DamageCollider>();
            LeftDamageCollider.characterManager = GetComponentInParent<CharacterManager>();
        }
        else {
            RightDamageCollider = rightHandSlot.CurrentWeaponModel.GetComponentInChildren<DamageCollider>();
            RightDamageCollider.characterManager = GetComponentInParent<CharacterManager>();
        }
    }

    //TODO: Implement these
    public void DrainStaminaLightAttack() { }
    public void DrainStaminaHeavyAttack() { }
    public void EnableCombo() { }
    public void DisableCombo() { }


    #region Handle Weapon's Damage Collider

    private void LoadLeftWeaponDamageCollider() {
        LeftDamageCollider =
            leftHandSlot.CurrentWeaponModel.GetComponentInChildren<DamageCollider>();
        LeftDamageCollider.characterManager = GetComponentInParent<CharacterManager>();
    }

    private void LoadRightWeaponDamageCollider() {
        RightDamageCollider =
            rightHandSlot.CurrentWeaponModel.GetComponentInChildren<DamageCollider>();
        RightDamageCollider.characterManager = GetComponentInParent<CharacterManager>();
    }

    public void OpenDamageCollider() {
        RightDamageCollider.EnableDamageCollider();
    }

    public void CloseDamageCollider() {
        RightDamageCollider.DisableDamageCollider();
    }
    
    //TODO: test if this actually works
    public void GrantWeaponAttackingPoiseBonus() {
        enemyStatsManager.TotalPoise += enemyStatsManager.OffensivePoise;
    }
    public void RemoveWeaponAttackingPoiseBonus() {
        enemyStatsManager.TotalPoise -= enemyStatsManager.OffensivePoise;
    }

    #endregion
}
}