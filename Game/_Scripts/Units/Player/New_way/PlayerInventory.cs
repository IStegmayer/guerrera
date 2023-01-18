using System;
using UnityEngine;

namespace Dzajna
{
    public class PlayerInventory : MonoBehaviour
    {
        public WeaponItem RightWeapon;
        public WeaponItem LeftWeapon;

        private WeaponSlotManager weaponSlotManager;
        
        private void Awake() {
            weaponSlotManager = GetComponent<WeaponSlotManager>();
        }

        private void Start() {
            weaponSlotManager.LoadWeaponOnSlot(RightWeapon, false);
            weaponSlotManager.LoadWeaponOnSlot(LeftWeapon, true);
        }
    }
}