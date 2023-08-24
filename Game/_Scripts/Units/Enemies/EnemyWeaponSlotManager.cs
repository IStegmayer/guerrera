using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dzajna {
public class EnemyWeaponSlotManager : CharacterWeaponSlotManager {
    private EnemyStatsManager enemyStatsManager;
    private CharacterEffectsManager characterEffectsManager;

    protected override void Awake() {
        base.Awake();
        enemyStatsManager = GetComponentInParent<EnemyStatsManager>();
        characterEffectsManager = GetComponentInParent<CharacterEffectsManager>();
        LoadWeaponHolderSlots();
    }

    protected void Start() {
        LoadWeaponOnSlot(rightHandWeapon, false);
        LoadWeaponOnSlot(leftHandWeapon, true);
    }

    private void LoadWeaponHolderSlots() {
        WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
        foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots) {
            if (weaponSlot.isLeftHandSlot) leftHandSlot = weaponSlot;
            else if (weaponSlot.isRightHandSlot) rightHandSlot = weaponSlot;
        }
    }

    public override void LoadWeaponOnSlot(WeaponItem weapon, bool isLeft) {
        WeaponItem weaponToEquip = weapon != null ? weapon : unarmedWeapon;

        if (isLeft && leftHandSlot != null) {
            leftHandSlot.CurrentWeaponItem = weaponToEquip;
            leftHandSlot.LoadWeaponModel(weaponToEquip);
            LoadLeftWeaponDamageCollider();
        }
        else {
            rightHandSlot.CurrentWeaponItem = weaponToEquip;
            rightHandSlot.LoadWeaponModel(weaponToEquip);
            LoadRightWeaponDamageCollider();
        }
    }

    private void LoadWeaponsDamageCollider(bool isLeft) {
        if (isLeft) {
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

    protected override void LoadLeftWeaponDamageCollider() {
        LeftDamageCollider =
            leftHandSlot.CurrentWeaponModel.GetComponentInChildren<DamageCollider>();
        LeftDamageCollider.characterManager = GetComponentInParent<CharacterManager>();

        characterEffectsManager.LeftWeaponFX = leftHandSlot.CurrentWeaponModel.GetComponentInChildren<WeaponFX>();
    }

    protected override void LoadRightWeaponDamageCollider() {
        WeaponItem weapon = rightHandSlot.CurrentWeaponItem;
        GameObject weaponModel = rightHandSlot.CurrentWeaponModel;

        RightDamageCollider = weaponModel.GetComponentInChildren<DamageCollider>();
        RightDamageCollider.characterManager = GetComponentInParent<CharacterManager>();
        RightDamageCollider.PhysicalDamage = weapon.BaseDamage;
        RightDamageCollider.PoiseBreak = weapon.PoiseBreak;
        RightDamageCollider.OffensivePoiseBonus = weapon.OffensivePoiseBonus;

        WeaponFX weaponFX = weaponModel.GetComponentInChildren<WeaponFX>();
        if (weaponFX != null) character.CharacterEffectsManager.RightWeaponFX = weaponFX;
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