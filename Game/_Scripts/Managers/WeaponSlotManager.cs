using System;
using UnityEngine;

namespace Dzajna
{
    public class WeaponSlotManager : MonoBehaviour
    {
        private WeaponHolderSlot leftHandSlot;
        private WeaponHolderSlot rightHandSlot;

        private DamageCollider leftDamageCollider;
        private DamageCollider rightDamageCollider;

        private Animator animator;

        private void Awake() {
            animator = GetComponent<Animator>();
            WeaponHolderSlot[] _weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
            foreach (WeaponHolderSlot _weaponSlot in _weaponHolderSlots) {
                if (_weaponSlot.isLeftHandSlot) leftHandSlot = _weaponSlot;
                else if (_weaponSlot.isRightHandSlot) rightHandSlot = _weaponSlot;
            }
        }

        public void LoadWeaponOnSlot(WeaponItem _weaponItem, bool _isLeft) {
            if (_isLeft) {
                leftHandSlot.LoadWeaponModel(_weaponItem);
                LoadLeftWeaponDamageCollider();
                if (_weaponItem != null) animator.CrossFade(_weaponItem.OH_LeftArmIdle, 0.2f);
                else animator.CrossFade("LeftArmEmpty", 0.2f);
            }
            else {
                rightHandSlot.LoadWeaponModel(_weaponItem);
                LoadRightWeaponDamageCollider();
                if (_weaponItem != null) animator.CrossFade(_weaponItem.OH_RightArmIdle, 0.2f);
                else animator.CrossFade("RightArmEmpty", 0.2f);
            }
        }

        #region Handle Weapon's Damage Collider
            private void LoadLeftWeaponDamageCollider() => leftDamageCollider =
                leftHandSlot.CurrentWeaponModel.GetComponentInChildren<DamageCollider>();

            private void LoadRightWeaponDamageCollider() => rightDamageCollider =
                rightHandSlot.CurrentWeaponModel.GetComponentInChildren<DamageCollider>();

            public void OpenLeftDamageCollider() => leftDamageCollider.EnableDamageCollider();
            public void OpenRightDamageCollider() => rightDamageCollider.EnableDamageCollider();
            public void CloseLeftDamageCollider() => leftDamageCollider.DisableDamageCollider();
            public void CloseRightDamageCollider() => rightDamageCollider.DisableDamageCollider();
        #endregion
    }
}