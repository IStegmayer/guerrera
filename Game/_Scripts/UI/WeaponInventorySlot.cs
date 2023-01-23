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
        
    }
}