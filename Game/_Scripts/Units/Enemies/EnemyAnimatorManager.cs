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
        }

        private void OnAnimatorMove() {
            float delta = Time.deltaTime;
            Vector3 _deltaPos = Anim.deltaPosition;
            _deltaPos.y = 0;
            Vector3 _velocity = _deltaPos / delta;
            
            rigidbody.drag = 0;
            rigidbody.velocity = _velocity;
        }
        
        public override void TakeCriticalDamageAnimEvent() {
            enemyStats.TakeDamageWithoutAnimation(enemyManager.PendingCriticalDamage);
            enemyManager.PendingCriticalDamage = 0;
        }
    }
}