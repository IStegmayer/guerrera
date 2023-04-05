using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dzajna
{
    public class WeaponInventorySlot : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private WeaponItem item;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private EquipmentWindowUI equipmentWindowUI;

        private PlayerInventoryManager playerInventoryManager;
        private PlayerWeaponSlotManager playerWeaponSlotManager;

        public void AddItem(WeaponItem _newItem) {
            item = _newItem;
            icon.sprite = item.ItemIcon;
            gameObject.SetActive(true);
            icon.enabled = true;
        }

        public void ClearInventorySlot() {
            item = null;
            icon.sprite = null;
            icon.enabled = false;
            gameObject.SetActive(false);
        }

        public void EquipThisItem() {
            //TODO: this is getting ridiculous with all these player references that cant be set on awake...
            if (playerInventoryManager == null) playerInventoryManager = FindObjectOfType<PlayerInventoryManager>();
            if (playerWeaponSlotManager == null) playerWeaponSlotManager = FindObjectOfType<PlayerWeaponSlotManager>();

            if (uiManager.RightHandSlot1Selected) {
                playerInventoryManager.WeaponsInventory.Add(playerInventoryManager.WeaponsInRightHandSlots[0]);
                playerInventoryManager.WeaponsInRightHandSlots[0] = item;
                playerInventoryManager.WeaponsInventory.Remove(item);
            }
            else if (uiManager.RightHandSlot2Selected) {
                playerInventoryManager.WeaponsInventory.Add(playerInventoryManager.WeaponsInRightHandSlots[1]);
                playerInventoryManager.WeaponsInRightHandSlots[1] = item;
                playerInventoryManager.WeaponsInventory.Remove(item);
            }
            else if (uiManager.LeftHandSlot1Selected) {
                playerInventoryManager.WeaponsInventory.Add(playerInventoryManager.WeaponsInLeftHandSlots[0]);
                playerInventoryManager.WeaponsInLeftHandSlots[0] = item;
                playerInventoryManager.WeaponsInventory.Remove(item);
            }
            else if (uiManager.LeftHandSlot2Selected) {
                playerInventoryManager.WeaponsInventory.Add(playerInventoryManager.WeaponsInLeftHandSlots[1]);
                playerInventoryManager.WeaponsInLeftHandSlots[1] = item;
                playerInventoryManager.WeaponsInventory.Remove(item);
            }
            else return;

            playerInventoryManager.RightWeapon =
                playerInventoryManager.WeaponsInRightHandSlots[playerInventoryManager.currentRightWeaponIndex];
            playerInventoryManager.LeftWeapon = playerInventoryManager.WeaponsInLeftHandSlots[playerInventoryManager.currentLeftWeaponIndex];
            
            playerWeaponSlotManager.LoadWeaponOnSlot(playerInventoryManager.RightWeapon, false);
            playerWeaponSlotManager.LoadWeaponOnSlot(playerInventoryManager.LeftWeapon, true);
            
            equipmentWindowUI.LoadWeaponsOnEquipmentScreen(playerInventoryManager);
            uiManager.ResetAllSelectedSlots();
        }
    }
}