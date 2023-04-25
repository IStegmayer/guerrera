using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dzajna {
public class PlayerEquipmentManager : MonoBehaviour {
    private PlayerInventoryManager playerInventoryManager;
    private BlockingCollider blockingCollider;

    private void Awake() {
        blockingCollider = GetComponentInChildren<BlockingCollider>();
        playerInventoryManager = GetComponent<PlayerInventoryManager>();
    }

    public void OpenBlockingCollider() {
        WeaponItem blockingWeapon = InputHandler.Instance.TwoHandFlag
            ? playerInventoryManager.RightWeapon
            : playerInventoryManager.LeftWeapon;
        blockingCollider.SetColliderDamageAbosorption(blockingWeapon);
        blockingCollider.EnableBlockingCollider();
    }

    public void CloseBlockingCollider() => blockingCollider.DisableBlockingCollider();
}
}