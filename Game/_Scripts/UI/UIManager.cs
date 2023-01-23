using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dzajna
{
    public class UIManager : MonoBehaviour
    {
        [Header("UI Windows")]
        [SerializeField] private GameObject HUDWindow;
        [SerializeField] private GameObject selectWindow;
        [SerializeField] private GameObject weaponInventoryWindow;

        [Header("Weapons Inventory")]
        [SerializeField] private Transform weaponInventorySlotsContainer;
        [SerializeField] private Transform weaponInventorySlotPrefab;
        private WeaponInventorySlot[] weaponInventorySlots;
        public PlayerInventory PlayerInventory;

        private void Start() {
            weaponInventorySlots = weaponInventorySlotsContainer.GetComponentsInChildren<WeaponInventorySlot>(true);
        }

        public void UpdateUI() {
            #region WeaponInventorySlots

            int _inventoryCount = PlayerInventory.WeaponsInventory.Count;
            for (int _i = 0; _i < weaponInventorySlots.Length; _i++) {
                if (_i < _inventoryCount) {
                    if (weaponInventorySlots.Length < _inventoryCount) {
                        Instantiate(weaponInventorySlotPrefab, weaponInventorySlotsContainer);
                        weaponInventorySlots =
                            weaponInventorySlotsContainer.GetComponentsInChildren<WeaponInventorySlot>(true);
                    }

                    weaponInventorySlots[_i].AddItem(PlayerInventory.WeaponsInventory[_i]);
                }
                // else {
                //     weaponInventorySlots[_i].ClearInventorySlot();
                // }
            }

            #endregion
        }

        public void ToggleSelectWindow(bool _active) => selectWindow.SetActive(_active);
        public void ToggleHUDWindow(bool _active) => HUDWindow.SetActive(_active);
        public void ToggleAllInventoryWindows(bool _active) => weaponInventoryWindow.SetActive(_active);
    }
}