using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicGoblin : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision) {
        Debug.Log("Collided" + collision);
        if(collision.gameObject.layer == LayerMask.NameToLayer("Weapon"))
            Destroy(this.gameObject);
    }
}
