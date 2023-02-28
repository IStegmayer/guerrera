using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dzajna {
public class BlockingCollider : MonoBehaviour {
    public float blockingPhysicalDmgAbsorption;
    private BoxCollider blockingCollider;

    private void Awake() {
        blockingCollider = GetComponent<BoxCollider>();
    }

    public void SetColliderDamageAbosorption(WeaponItem _weapon) {
        if (_weapon != null) blockingPhysicalDmgAbsorption = _weapon.pshysicalDmgAbsorption;
    }

    public void EnableBlockingCollider() => blockingCollider.enabled = true;
    public void DisableBlockingCollider() => blockingCollider.enabled = false;
}

}
