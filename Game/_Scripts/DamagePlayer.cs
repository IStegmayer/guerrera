using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Dzajna
{
    public class DamagePlayer : MonoBehaviour
    {
        [SerializeField] private int damage = 10;

        private void OnTriggerEnter(Collider _other) {
            PlayerStats _playerStats = _other.GetComponent<PlayerStats>();
            
            if (_playerStats != null) _playerStats.TakeDamage(damage);
        }
    }
}
