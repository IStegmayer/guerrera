using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

namespace Dzajna
{
    public class DamageCollider : MonoBehaviour
    {
        private Collider damageCollider;
        public float CurrentWeaponDamage = 25;

        private void Awake() {
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.isTrigger = true;
            damageCollider.enabled = false;
        }

        public void EnableDamageCollider() => damageCollider.enabled = true;
        public void DisableDamageCollider() => damageCollider.enabled = false;

        private void OnTriggerEnter(Collider _collider) {
            //TODO: dont use a tag for this, maybe an interface?
            
            if (_collider.gameObject.CompareTag("Player")) {
                PlayerStats _playerStats = _collider.GetComponent<PlayerStats>();
                if (_playerStats != null) {
                    _playerStats.TakeDamage(CurrentWeaponDamage);
                }
            }
            if (_collider.gameObject.CompareTag("Enemy")) {
                EnemyStats _enemyStats = _collider.GetComponent<EnemyStats>();
                if (_enemyStats != null) {
                    _enemyStats.TakeDamage(CurrentWeaponDamage);
                }
            }
        }
    }
}