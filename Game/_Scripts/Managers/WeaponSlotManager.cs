using System;
using UnityEngine;

namespace Dzajna {
public class WeaponSlotManager : MonoBehaviour {
    private WeaponHolderSlot leftHandSlot;
    private WeaponHolderSlot rightHandSlot;
    private WeaponHolderSlot backSlot;

    public DamageCollider LeftDamageCollider { get; private set; }
    public DamageCollider RightDamageCollider { get; private set; }

    private WeaponItem attackingWeapon;
    public float CurrentWeaponDamage;

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
        WeaponHolderSlot[] _weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
        foreach (WeaponHolderSlot _weaponSlot in _weaponHolderSlots) {
            if (_weaponSlot.isLeftHandSlot) leftHandSlot = _weaponSlot;
            else if (_weaponSlot.isRightHandSlot) rightHandSlot = _weaponSlot;
            else if (_weaponSlot.isBackSlot) backSlot = _weaponSlot;
        }
    }

    public void LoadWeaponOnSlot(WeaponItem _weaponItem, bool _isLeft) {
        if (_isLeft) {
            leftHandSlot.CurrentWeaponItem = _weaponItem;
            leftHandSlot.LoadWeaponModel(_weaponItem);
            LoadLeftWeaponDamageCollider();
            quickSlotsUI.UpdateWeaponQuickSlotsUI(_weaponItem, true);
            //TODO: hashes
            if (_weaponItem != null) animator.CrossFade(_weaponItem.OH_LeftArmIdle, 0.2f);
            else animator.CrossFade("LeftArmEmpty", 0.2f);
        }
        else {
            if (inputHandler.TwoHandFlag) {
                backSlot.LoadWeaponModel(leftHandSlot.CurrentWeaponItem);
                animator.CrossFade(_weaponItem.TH_Idle, 0.2f);
                leftHandSlot.UnloadWeaponAndDestroy();
            }
            else {
                //TODO: hashes
                animator.CrossFade("BothArmsEmpty", 0.2f);
                if (_weaponItem != null) animator.CrossFade(_weaponItem.OH_RightArmIdle, 0.2f);
                else animator.CrossFade("RightArmEmpty", 0.2f);

                backSlot.UnloadWeaponAndDestroy();
            }

            rightHandSlot.CurrentWeaponItem = _weaponItem;
            rightHandSlot.LoadWeaponModel(_weaponItem);
            LoadRightWeaponDamageCollider();
            quickSlotsUI.UpdateWeaponQuickSlotsUI(_weaponItem, false);
        }
    }

    #region Handle Weapon's Damage Collider

    private void LoadLeftWeaponDamageCollider() {
        LeftDamageCollider =
            leftHandSlot.CurrentWeaponModel.GetComponentInChildren<DamageCollider>();
        LeftDamageCollider.CurrentWeaponDamage = leftHandSlot.CurrentWeaponItem.BaseDamage;
    }

    private void LoadRightWeaponDamageCollider() {
        RightDamageCollider =
            rightHandSlot.CurrentWeaponModel.GetComponentInChildren<DamageCollider>();
        RightDamageCollider.CurrentWeaponDamage = rightHandSlot.CurrentWeaponItem.BaseDamage;
    } 

    public void OpenDamageCollider() {
        if (playerManager.IsUsingLeftHand)
            LeftDamageCollider.EnableDamageCollider();
        else if (playerManager.IsUsingRightHand)
            RightDamageCollider.EnableDamageCollider();
    }

    public void CloseDamageCollider() {
        LeftDamageCollider.DisableDamageCollider();
        RightDamageCollider.DisableDamageCollider();
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

    public Transform GetRightHandTransform() => rightHandSlot.transform;
}
}