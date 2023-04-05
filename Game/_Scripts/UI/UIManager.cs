using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dzajna
{
    public class UIManager : MonoBehaviour
    {
        public PlayerInventoryManager _playerInventoryManager;
        
        [Header("UI Windows")]
        [SerializeField] private GameObject HUDWindow;
        [SerializeField] private GameObject selectWindow;
        [SerializeField] private GameObject weaponInventoryWindow;
        [SerializeField] private GameObject equipmentWindow;

        [Header("Equipment Window Slot Selected")]
        public bool RightHandSlot1Selected;
        public bool RightHandSlot2Selected;
        public bool LeftHandSlot1Selected;
        public bool LeftHandSlot2Selected;

        [Header("Weapons Inventory")]
        [SerializeField] private Transform weaponInventorySlotsContainer;
        [SerializeField] private Transform weaponInventorySlotPrefab;
        [SerializeField] private EquipmentWindowUI equipmentWindowUI;
        private WeaponInventorySlot[] weaponInventorySlots;

        private void Awake() {
            equipmentWindowUI = GetComponentInChildren<EquipmentWindowUI>(true);
        }

        private void Start() {
            weaponInventorySlots = weaponInventorySlotsContainer.GetComponentsInChildren<WeaponInventorySlot>(true);
            equipmentWindowUI.LoadWeaponsOnEquipmentScreen(_playerInventoryManager);
        }

        public void UpdateUI() {
            #region WeaponInventorySlots

            int _inventoryCount = _playerInventoryManager.WeaponsInventory.Count;
            for (int _i = 0; _i < weaponInventorySlots.Length; _i++) {
                if (_i < _inventoryCount) {
                    if (weaponInventorySlots.Length < _inventoryCount) {
                        Instantiate(weaponInventorySlotPrefab, weaponInventorySlotsContainer);
                        weaponInventorySlots =
                            weaponInventorySlotsContainer.GetComponentsInChildren<WeaponInventorySlot>(true);
                    }

                    weaponInventorySlots[_i].AddItem(_playerInventoryManager.WeaponsInventory[_i]);
                }
                else {
                    weaponInventorySlots[_i].ClearInventorySlot();
                }
            }

            #endregion
        }

        public void ToggleSelectWindow(bool _active) => selectWindow.SetActive(_active);
        public void ToggleHUDWindow(bool _active) => HUDWindow.SetActive(_active);
        public void CloseAllInventoryWindows() {
            ResetAllSelectedSlots();
            weaponInventoryWindow.SetActive(false);
            equipmentWindow.SetActive(false);
        }
        
        public void ResetAllSelectedSlots() {
            RightHandSlot1Selected = false;
            RightHandSlot2Selected = false;
            LeftHandSlot1Selected = false;
            LeftHandSlot2Selected = false;
        }
    }
}