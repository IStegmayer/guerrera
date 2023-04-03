using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Dzajna {
public class AttackState : State {
    [SerializeField] private CombatStanceState combatStanceState;
    [SerializeField] private PursueTargetState pursueTargetState;
    [SerializeField] private RotateTowardsTargetState rotateTowardsTargetState;
    public EnemyAttackAction CurrentAttack;

    private bool willComboNextAttack;
    private bool hasPerformedAttack;

    public override State Tick(EnemyManager _enemyManager, EnemyStats _enemyStats,
        EnemyAnimatorManager _enemyAnimatorM) {
        if (_enemyManager.IsInteracting) return this;
        
        Vector3 _targetPos = _enemyManager.CurrentTarget.transform.position;
        Vector3 _targetsDirection = _targetPos - _enemyManager.transform.position;
        float _distanceFromTarget = Vector3.Distance(_targetPos, _enemyManager.transform.position);

        RotateTowardsTargetWhilstAttacking(_enemyManager, _targetsDirection);

        if (_distanceFromTarget > _enemyManager.MaxAggroRadius) return pursueTargetState;

        if (willComboNextAttack && _enemyManager.CanDoCombo) {
            AttackTargetWithCombo(_enemyManager, _enemyAnimatorM);
            _enemyManager.CurrentRecoveryTime = CurrentAttack.RecoveryTime;
        }

        if (!hasPerformedAttack) {
            AttackTarget(_enemyManager, _enemyAnimatorM);
            RollForComboChance(_enemyManager);
        }

        if (willComboNextAttack && hasPerformedAttack) {
            return this; // Go back up to perform combo
        }

        return rotateTowardsTargetState;
    }

    private void AttackTarget(EnemyManager _enemyManager, EnemyAnimatorManager _enemyAnimatorManager) {
        _enemyAnimatorManager.PlayTargetAnimation(CurrentAttack.ActionAnimation, true);
        _enemyManager.CurrentRecoveryTime = CurrentAttack.RecoveryTime; // if comboing this will be ignored
        hasPerformedAttack = true;
    }

    private void AttackTargetWithCombo(EnemyManager _enemyManager, EnemyAnimatorManager _enemyAnimatorManager) {
        willComboNextAttack = false;
        _enemyAnimatorManager.PlayTargetAnimation(CurrentAttack.ActionAnimation, true);
        _enemyManager.CurrentRecoveryTime = CurrentAttack.RecoveryTime;
        hasPerformedAttack = true;
    }

    private void RollForComboChance(EnemyManager _enemyManager) {
        float _comboChance = Random.Range(0, 100);

        if (!_enemyManager.AllowAIToPerformCombos || _comboChance > _enemyManager.ComboChance) return;

        if (CurrentAttack.comboAction != null) {
            willComboNextAttack = true;
            CurrentAttack = CurrentAttack.comboAction;
        }
        else {
            willComboNextAttack = false;
            CurrentAttack = null;
        }
    }

    private void RotateTowardsTargetWhilstAttacking(EnemyManager _enemyManager, Vector3 _direction) {
        if (_enemyManager.CanRotate) {
            _direction.y = 0;
            _direction.Normalize();

            if (_direction == Vector3.zero) _direction = _enemyManager.transform.forward;

            Quaternion _targetRotation = Quaternion.LookRotation(_direction);
            _enemyManager.transform.rotation =
                Quaternion.Slerp(transform.rotation, _targetRotation,
                    _enemyManager.RotationSpeed / Time.fixedDeltaTime);
        }
    }

    public void SetHasPerformedAttack(bool _value) => hasPerformedAttack = _value;
}
}