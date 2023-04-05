using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dzajna {
public class PlayerEquipmentManager : MonoBehaviour {
    private InputHandler inputHandler;
    private PlayerInventoryManager playerInventoryManager;
    private BlockingCollider blockingCollider;
    
    private void Awake() {
        blockingCollider = GetComponentInChildren<BlockingCollider>();
        playerInventoryManager = GetComponent<PlayerInventoryManager>();
        inputHandler = GetComponent<InputHandler>();
    }

    public void OpenBlockingCollider() {
        WeaponItem _blockingWeapon = inputHandler.TwoHandFlag ? playerInventoryManager.RightWeapon : playerInventoryManager.LeftWeapon;
        blockingCollider.SetColliderDamageAbosorption(_blockingWeapon);
        blockingCollider.EnableBlockingCollider();
    }

    public void CloseBlockingCollider() => blockingCollider.DisableBlockingCollider();

}

}
