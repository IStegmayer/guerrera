using System;
using UnityEngine;

public class EnemyMeleeDamageDealer : MonoBehaviour
{
    private BaseUnit _enemy;

    //TODO: This should be a class managed by the Unit itself using scriptableObjects
    // but this'll do for now
    [SerializeField] private int _weaponDmg = 1;
    private bool _canDealDamage;

    public void ToggleEnableDamage(bool state) => _canDealDamage = state;

    private void Awake() {
        _enemy = transform.root.gameObject.GetComponentInChildren<BasicGoblin>();
    }

    private void OnTriggerEnter(Collider other) {
        if (!_canDealDamage) return;
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
        if (!other.gameObject.TryGetComponent(out BaseUnit player)) return;
        
        player.TakeDamage(_enemy.Stats.BaseAttackPower + _weaponDmg);
        _canDealDamage = false;
    }
}