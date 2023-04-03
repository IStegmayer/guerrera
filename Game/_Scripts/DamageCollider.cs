using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

namespace Dzajna {
public class DamageCollider : MonoBehaviour {
    public CharacterManager characterManager;
    public bool EnabledOnStartup;

    protected Collider damageCollider;
    [Header("Damage")]
    public float CurrentWeaponDamage = 25;

    [Header("Poise")] 
    public float PoiseBreak = 10;
    public float OffensivePoiseBonus = 10;

    private void Awake() {
        damageCollider = GetComponent<Collider>();
        damageCollider.gameObject.SetActive(true);
        damageCollider.isTrigger = true;
        damageCollider.enabled = EnabledOnStartup;
    }

    public void EnableDamageCollider() => damageCollider.enabled = true;
    public void DisableDamageCollider() => damageCollider.enabled = false;

    private void OnTriggerEnter(Collider _collider) {
        //TODO: dont use a tag for this, maybe an interface?

        if (_collider.gameObject.CompareTag("Player")) {
            PlayerStats _playerStats = _collider.GetComponent<PlayerStats>();
            CharacterManager _enemyCharacterManager = _collider.GetComponent<CharacterManager>();
            BlockingCollider _shield = _collider.GetComponentInChildren<BlockingCollider>(true);
            AnimatorManager _anim = characterManager.GetComponent<AnimatorManager>();
            float _dmgToBeDealt = CurrentWeaponDamage;

            if (_enemyCharacterManager == null) throw new Exception("No enemy character manager found.");
            if (_playerStats == null) throw new Exception("No player stats manager found.");

            if (_enemyCharacterManager.IsParrying) {
                // CHECK IF YOU ARE PARRIABLE
                _anim.PlayTargetAnimation(_anim.ParriedHash, true);
                return;
            }

            if (_shield != null && _enemyCharacterManager.IsBlocking) {
                _dmgToBeDealt = CurrentWeaponDamage -
                                (CurrentWeaponDamage * _shield.blockingPhysicalDmgAbsorption) / 100;
            }

            string _dmgAnim = _enemyCharacterManager.IsBlocking ? "BlockDamage1" : "Damage1";
            _playerStats.ResetPoiseTimer();
            _playerStats.DealPoiseDamage(PoiseBreak);
            if (_playerStats.TotalPoise > PoiseBreak) {
                _playerStats.TakeDamageWithoutAnimation(CurrentWeaponDamage);
                Debug.Log("Enemy's poise is: " + _playerStats.TotalPoise);
            }
            else _playerStats.TakeDamage(CurrentWeaponDamage);
        }

        if (_collider.gameObject.CompareTag("Enemy")) {
            //TODO: add blocking
            EnemyStats _enemyStats = _collider.GetComponent<EnemyStats>();
            CharacterManager _enemyCharacterManager = _collider.GetComponent<CharacterManager>();
            if (_enemyCharacterManager != null && _enemyCharacterManager.IsParrying) {
                // CHECK IF YOU ARE PARRIABLE
                AnimatorManager _anim = characterManager.GetComponent<AnimatorManager>();
                _anim.PlayTargetAnimation(_anim.ParriedHash, true);
                return;
            }

            if (_enemyStats != null) {
                _enemyStats.ResetPoiseTimer();
                _enemyStats.DealPoiseDamage(PoiseBreak);
                if (_enemyStats.TotalPoise > PoiseBreak) {
                    _enemyStats.TakeDamageWithoutAnimation(CurrentWeaponDamage);
                    Debug.Log("Enemy's poise is: " + _enemyStats.TotalPoise);
                }
                else _enemyStats.TakeDamage(CurrentWeaponDamage);
            }
        }
    }
}
}