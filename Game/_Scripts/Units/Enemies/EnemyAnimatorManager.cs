using System;
using UnityEngine;

namespace Dzajna
{
    public class EnemyAnimatorManager : AnimatorManager
    {
        private Rigidbody rigidbody;
        private EnemyStats enemyStats;
        private EnemyManager enemyManager;

        public int PhaseTransitionHash;
        public int IsPhaseShiftingHash;
        
        private void Awake() {
            Anim = GetComponent<Animator>();
            rigidbody = GetComponentInChildren<Rigidbody>();
            enemyManager = GetComponent<EnemyManager>();
            enemyStats = GetComponent<EnemyStats>();
            VerticalHash = Animator.StringToHash("Vertical");
            HorizontalHash = Animator.StringToHash("Horizontal");
            IsInteractingHash = Animator.StringToHash("isInteracting");
            IsDeadHash = Animator.StringToHash("isDead");
            CanRotateHash = Animator.StringToHash("canRotate");
            ParriedHash = Animator.StringToHash("Parried");
            ParryHash = Animator.StringToHash("Parry");
            CanDoComboHash = Animator.StringToHash("canDoCombo");
            IsInvulnerableHash = Animator.StringToHash("isInvulnerable");
            CanRotateHash = Animator.StringToHash("canRotate");
            IsRotatingWithRMHash = Animator.StringToHash("isRotatingWithRM");
            TurnAroundHash = Animator.StringToHash("TurnAround");
            TurnLeftHash = Animator.StringToHash("TurnLeft");
            TurnRightHash = Animator.StringToHash("TurnRight");
            PhaseTransitionHash = Animator.StringToHash("PhaseTransition");
            IsPhaseShiftingHash = Animator.StringToHash("isPhaseShifting");
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
            if (_playerStats != null) _playerStats.AddSouls(enemyStats.SoulsReward);
            if (_soulCount != null) _soulCount.SetSoulCountText(_playerStats.SoulCount);
        }
        public override void TakeCriticalDamageAnimEvent() {
            enemyStats.TakeDamageWithoutAnimation(enemyManager.PendingCriticalDamage);
            enemyManager.PendingCriticalDamage = 0;
        }
    }
}