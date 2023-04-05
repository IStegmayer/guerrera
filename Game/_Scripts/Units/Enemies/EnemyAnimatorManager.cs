using System;
using UnityEngine;

namespace Dzajna
{
    public class EnemyAnimatorManager : AnimatorManager
    {
        private Rigidbody rigidbody;
        private EnemyStatsManager enemyStatsManager;
        private EnemyManager enemyManager;

        
        protected override void Awake() {
            base.Awake();
            Anim = GetComponent<Animator>();
            rigidbody = GetComponentInChildren<Rigidbody>();
            enemyManager = GetComponent<EnemyManager>();
            enemyStatsManager = GetComponent<EnemyStatsManager>();
        }

        private void OnAnimatorMove() {
            float delta = Time.deltaTime;
            Vector3 _deltaPos = Anim.deltaPosition;
            _deltaPos.y = 0;
            Vector3 _velocity = _deltaPos / delta;
            
            rigidbody.drag = 0;
            rigidbody.velocity = _velocity;

            if (enemyManager.IsRotatingWithRM) enemyManager.transform.rotation *= Anim.deltaRotation;
        }
        
        //TODO: this is not the best to communicate between scripts for sure, use unitmanager instead! 
        public void AwardSoulsOnDeath() {
            PlayerStats _playerStats = FindObjectOfType<PlayerStats>();
            SoulCountUI _soulCount = FindObjectOfType<SoulCountUI>();
            if (_playerStats != null) _playerStats.AddSouls(enemyStatsManager.SoulsReward);
            if (_soulCount != null) _soulCount.SetSoulCountText(_playerStats.SoulCount);
        }
        public override void TakeCriticalDamageAnimEvent() {
            enemyStatsManager.TakeDamageWithoutAnimation(enemyManager.PendingCriticalDamage);
            enemyManager.PendingCriticalDamage = 0;
        }
    }
}