using System;
using UnityEngine;

namespace Dzajna
{
    public class EnemyAnimatorManager : AnimatorManager
    {
        private Rigidbody rigidbody;
        private EnemyStats enemyStats;
        private EnemyManager enemyManager;

        private void Awake() {
            Anim = GetComponent<Animator>();
            rigidbody = GetComponentInChildren<Rigidbody>();
            enemyManager = GetComponent<EnemyManager>();
            enemyStats = GetComponent<EnemyStats>();
            IsInteractingHash = Animator.StringToHash("isInteracting");
            IsDeadHash = Animator.StringToHash("isDead");
            CanRotateHash = Animator.StringToHash("canRotate");
            ParriedHash = Animator.StringToHash("Parried");
            ParryHash = Animator.StringToHash("Parry");
        }

        private void OnAnimatorMove() {
            float delta = Time.deltaTime;
            Vector3 _deltaPos = Anim.deltaPosition;
            _deltaPos.y = 0;
            Vector3 _velocity = _deltaPos / delta;
            
            rigidbody.drag = 0;
            rigidbody.velocity = _velocity;
        }
        
        //TODO: this is not the best to communicate between scripts for sure, use unitmanager instead! 
        public void AwardSoulsOnDeath() {
            PlayerStats _playerStats = FindObjectOfType<PlayerStats>();
            SoulCountUI _soulCount = FindObjectOfType<SoulCountUI>();
            if (_playerStats != null) _playerStats.AddSouls(enemyStats.SoulsReward);
            if (_soulCount != null) _soulCount.SetSoulCountText(_playerStats.SoulCount);
        }
        public void EnableIsParrying() => enemyManager.IsParrying = true;
        public void DisableIsParrying() => enemyManager.IsParrying = false;
        public void EnableCanBeRiposted() => enemyManager.CanBeRiposted = true;
        public void DisableCanBeRiposted() => enemyManager.CanBeRiposted = false;
        public override void TakeCriticalDamageAnimEvent() {
            enemyStats.TakeDamageWithoutAnimation(enemyManager.PendingCriticalDamage);
            enemyManager.PendingCriticalDamage = 0;
        }
    }
}