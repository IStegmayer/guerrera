using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dzajna {
public class PlayerEquipmentManager : MonoBehaviour {
    private InputHandler inputHandler;
    private PlayerInventory playerInventory;
    private BlockingCollider blockingCollider;
    
    private void Awake() {
        blockingCollider = GetComponentInChildren<BlockingCollider>();
        playerInventory = GetComponent<PlayerInventory>();
        inputHandler = GetComponent<InputHandler>();
    }

    public void OpenBlockingCollider() {
        WeaponItem _blockingWeapon = inputHandler.TwoHandFlag ? playerInventory.RightWeapon : playerInventory.LeftWeapon;
        blockingCollider.SetColliderDamageAbosorption(_blockingWeapon);
        blockingCollider.EnableBlockingCollider();
    }

    public void CloseBlockingCollider() => blockingCollider.DisableBlockingCollider();

}

}
