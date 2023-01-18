using System;
using UnityEngine;

namespace Dzajna
{
    public class WeaponSlotManager : MonoBehaviour
    {
        private WeaponHolderSlot leftHandSlot;
        private WeaponHolderSlot rightHandSlot;

        private Animator animator;
        
        private void Awake() {
            animator = GetComponent<Animator>();
            WeaponHolderSlot[] _weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
            foreach (WeaponHolderSlot _weaponSlot in _weaponHolderSlots) {
                if (_weaponSlot.isLeftHandSlot) leftHandSlot = _weaponSlot;
                else if (_weaponSlot.isRightHandSlot) rightHandSlot = _weaponSlot;
            }
        }

        public void LoadWeaponOnSlot(WeaponItem _weaponItem, bool isLeft) {
            if(isLeft) leftHandSlot.LoadWeaponModel(_weaponItem);
            else rightHandSlot.LoadWeaponModel(_weaponItem);
        }
    }
}