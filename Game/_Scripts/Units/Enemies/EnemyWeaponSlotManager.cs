using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dzajna {
public class EnemyWeaponSlotManager : MonoBehaviour {
    [SerializeField] private WeaponItem rightHandWeapon;
    [SerializeField] private WeaponItem leftHandWeapon;

    private WeaponHolderSlot rightHandSlot;
    private WeaponHolderSlot leftHandSlot;

    private DamageCollider rightDamageCollider;
    private DamageCollider leftDamageCollider;

    private EnemyStats enemyStats;

    private void Awake() {
        enemyStats = GetComponentInParent<EnemyStats>();
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
        if (_isLeft) {
            leftHandSlot.CurrentWeaponItem = _weapon;
            leftHandSlot.LoadWeaponModel(_weapon);
            LoadLeftWeaponDamageCollider();
        }
        else {
            rightHandSlot.CurrentWeaponItem = _weapon;
            rightHandSlot.LoadWeaponModel(_weapon);
            LoadRightWeaponDamageCollider();
        }
    }

    private void LoadWeaponsDamageCollider(bool _isLeft) {
        if (_isLeft) {
            leftDamageCollider = leftHandSlot.CurrentWeaponModel.GetComponentInChildren<DamageCollider>();
            leftDamageCollider.characterManager = GetComponentInParent<CharacterManager>();
        }
        else {
            rightDamageCollider = rightHandSlot.CurrentWeaponModel.GetComponentInChildren<DamageCollider>();
            rightDamageCollider.characterManager = GetComponentInParent<CharacterManager>();
        }
    }

    //TODO: Implement these
    public void DrainStaminaLightAttack() { }
    public void DrainStaminaHeavyAttack() { }
    public void EnableCombo() { }
    public void DisableCombo() { }


    #region Handle Weapon's Damage Collider

    private void LoadLeftWeaponDamageCollider() {
        leftDamageCollider =
            leftHandSlot.CurrentWeaponModel.GetComponentInChildren<DamageCollider>();
        leftDamageCollider.characterManager = GetComponentInParent<CharacterManager>();
    }

    private void LoadRightWeaponDamageCollider() {
        rightDamageCollider =
            rightHandSlot.CurrentWeaponModel.GetComponentInChildren<DamageCollider>();
        rightDamageCollider.characterManager = GetComponentInParent<CharacterManager>();
    }

    public void OpenDamageCollider() {
        rightDamageCollider.EnableDamageCollider();
    }

    public void CloseDamageCollider() {
        rightDamageCollider.DisableDamageCollider();
    }
    
    //TODO: test if this actually works
    public void GrantWeaponAttackingPoiseBonus() {
        enemyStats.TotalPoise += enemyStats.OffensivePoise;
    }
    public void RemoveWeaponAttackingPoiseBonus() {
        enemyStats.TotalPoise -= enemyStats.OffensivePoise;
    }

    #endregion
}
}