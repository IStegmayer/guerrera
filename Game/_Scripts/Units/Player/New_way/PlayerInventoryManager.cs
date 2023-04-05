using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dzajna
{
    public class PlayerInventoryManager : MonoBehaviour {
        public SpellItem CurrentSpell;
        public WeaponItem RightWeapon;
        public WeaponItem LeftWeapon;
        public WeaponItem UnarmedWeapon;

        public WeaponItem[] WeaponsInRightHandSlots = new WeaponItem[1];
        public WeaponItem[] WeaponsInLeftHandSlots = new WeaponItem[1];
        public int currentRightWeaponIndex = 0;
        public int currentLeftWeaponIndex = 0;

        public List<WeaponItem> WeaponsInventory = new List<WeaponItem>();

        private PlayerWeaponSlotManager playerWeaponSlotManager;

        private void Awake() {
            playerWeaponSlotManager = GetComponent<PlayerWeaponSlotManager>();
        }

        private void Start() {
            LeftWeapon = WeaponsInLeftHandSlots[0];
            RightWeapon = WeaponsInRightHandSlots[0];
            playerWeaponSlotManager.LoadWeaponOnSlot(LeftWeapon, true);
            playerWeaponSlotManager.LoadWeaponOnSlot(RightWeapon, false);
        }

        public void ChangeRightWeapon() {
            currentRightWeaponIndex++;
            if (currentRightWeaponIndex < WeaponsInRightHandSlots.Length &&
                WeaponsInRightHandSlots[currentRightWeaponIndex] != null) {
                LoadRightWeapon(WeaponsInRightHandSlots[currentRightWeaponIndex]);
            }
            else {
                LoadRightWeapon(UnarmedWeapon);
                currentRightWeaponIndex = -1;
            }
        }

        private void LoadRightWeapon(WeaponItem _weapon) {
            RightWeapon = _weapon;
            playerWeaponSlotManager.LoadWeaponOnSlot(_weapon, false);
        }
        
        public void ChangeLeftWeapon() {
            currentLeftWeaponIndex++;
            if (currentLeftWeaponIndex < WeaponsInLeftHandSlots.Length &&
                WeaponsInLeftHandSlots[currentLeftWeaponIndex] != null) {
                LoadLeftWeapon(WeaponsInLeftHandSlots[currentLeftWeaponIndex]);
            }
            else {
                LoadLeftWeapon(UnarmedWeapon);
                currentLeftWeaponIndex = -1;
            }
        }
        private void LoadLeftWeapon(WeaponItem _weapon) {
            LeftWeapon = _weapon;
            playerWeaponSlotManager.LoadWeaponOnSlot(_weapon, true);
        }
    }
}