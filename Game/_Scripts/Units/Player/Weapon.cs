using System;
using Game._Scripts;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private BaseUnit _player;
    //TODO: This should be a class managed by the Player itself using scriptableObjects
    // but this'll do for now
    private int _weaponDmg = 2;
    private bool _canDealDamage;

    public void ToggleEnableDamage(bool state) => _canDealDamage = state;
    
    private void Awake() {
        _player = transform.root.gameObject.GetComponentInChildren<BaseUnit>();
    }

    private void OnCollisionEnter(Collision other) {
        if (!_canDealDamage) return;
        
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            BaseUnit enemy = other.gameObject.GetComponent<BaseUnit>();
            enemy.TakeDamage(_player.Stats.BaseAttackPower + _weaponDmg);
        }

        _canDealDamage = false;
    }
}