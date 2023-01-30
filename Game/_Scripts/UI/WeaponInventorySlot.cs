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

        private PlayerInventory playerInventory;
        private WeaponSlotManager weaponSlotManager;

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
            if (playerInventory == null) playerInventory = FindObjectOfType<PlayerInventory>();
            if (weaponSlotManager == null) weaponSlotManager = FindObjectOfType<WeaponSlotManager>();

            if (uiManager.RightHandSlot1Selected) {
                playerInventory.WeaponsInventory.Add(playerInventory.WeaponsInRightHandSlots[0]);
                playerInventory.WeaponsInRightHandSlots[0] = item;
                playerInventory.WeaponsInventory.Remove(item);
            }
            else if (uiManager.RightHandSlot2Selected) {
                playerInventory.WeaponsInventory.Add(playerInventory.WeaponsInRightHandSlots[1]);
                playerInventory.WeaponsInRightHandSlots[1] = item;
                playerInventory.WeaponsInventory.Remove(item);
            }
            else if (uiManager.LeftHandSlot1Selected) {
                playerInventory.WeaponsInventory.Add(playerInventory.WeaponsInLeftHandSlots[0]);
                playerInventory.WeaponsInLeftHandSlots[0] = item;
                playerInventory.WeaponsInventory.Remove(item);
            }
            else if (uiManager.LeftHandSlot2Selected) {
                playerInventory.WeaponsInventory.Add(playerInventory.WeaponsInLeftHandSlots[1]);
                playerInventory.WeaponsInLeftHandSlots[1] = item;
                playerInventory.WeaponsInventory.Remove(item);
            }
            else return;

            playerInventory.RightWeapon =
                playerInventory.WeaponsInRightHandSlots[playerInventory.currentRightWeaponIndex];
            playerInventory.LeftWeapon = playerInventory.WeaponsInLeftHandSlots[playerInventory.currentLeftWeaponIndex];
            
            weaponSlotManager.LoadWeaponOnSlot(playerInventory.RightWeapon, false);
            weaponSlotManager.LoadWeaponOnSlot(playerInventory.LeftWeapon, true);
            
            equipmentWindowUI.LoadWeaponsOnEquipmentScreen(playerInventory);
            uiManager.ResetAllSelectedSlots();
        }
    }
}