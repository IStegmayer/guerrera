using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dzajna
{
    public class EquipmentSlotUI : MonoBehaviour
    {
        public WeaponItem weaponItem;
        [SerializeField] private Image icon;
        public bool RightHandSlot1;
        public bool RightHandSlot2;
        public bool LeftHandSlot1;
        public bool LeftHandSlot2;

        public void AddItem(WeaponItem _weaponItem) {
            weaponItem = _weaponItem;
            icon.sprite = _weaponItem.ItemIcon;
            icon.enabled = true;
            gameObject.SetActive(true);
        }
        public void ClearItem() {
            weaponItem = null;
            icon.sprite = null;
            icon.enabled = false;
            gameObject.SetActive(false);
        }
    }
}