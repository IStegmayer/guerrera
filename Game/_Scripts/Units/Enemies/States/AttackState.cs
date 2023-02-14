using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Dzajna {
public class AttackState : State {
    [SerializeField] private CombatStanceState combatStanceState;
    public EnemyAttackAction[] EnemyAttacks;
    public EnemyAttackAction CurrentAttack;

    public override State Tick(EnemyManager _enemyManager, EnemyStats _enemyStats,
        EnemyAnimatorManager _enemyAnimatorManager) {
        // Select one of our many attacks based on attack scores
        // If the selected attack is not able to be used because of bad angle or distance, select a new attack
        // If the attack is viable, stop our movement and attack our target
        // Set our recovery timer to the attacks recovery time
        // Return to the combat stance state
        Vector3 _targetPos = _enemyManager.CurrentTarget.transform.position;
        Vector3 _targetsDirection = _targetPos - _enemyManager.transform.position;
        HandleRotateTowardsTarget(_enemyManager, _targetsDirection);
        
        if (_enemyManager.IsPerformingAction) return combatStanceState;

        float _distanceFromTarget = Vector3.Distance(_targetPos, _enemyManager.transform.position);
        float _viewableAngle = Vector3.Angle(_targetsDirection, _enemyManager.transform.forward);

        if (CurrentAttack != null) {
            // If we are too close to the enemy to perform current attack, get a new attack
            if (_distanceFromTarget < CurrentAttack.MinDistanceNeededToAttack) return this;
            // If we are close enough to attack, then let us proceed
            else if (_distanceFromTarget < CurrentAttack.MaxDistanceNeededToAttack) {
                // If our enemy is within our attacks viewable angle, we attack
                if (_viewableAngle <= CurrentAttack.MaxAttackAngle &&
                    _viewableAngle >= CurrentAttack.MinAttackAngle) {
                    if (_enemyManager.CurrentRecoveryTime <= 0 && !_enemyManager.IsPerformingAction) {
                        _enemyAnimatorManager.Anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                        _enemyAnimatorManager.Anim.SetFloat("Horizontal", 0, 0.1f, Time.deltaTime);
                        _enemyAnimatorManager.PlayTargetAnimation(CurrentAttack.ActionAnimation, true);
                        _enemyManager.IsPerformingAction = true;
                        _enemyManager.CurrentRecoveryTime = CurrentAttack.RecoveryTime;
                        CurrentAttack = null;
                        return combatStanceState;
                    }
                }
            }
        }
        else GetNewAttack(_enemyManager, _targetPos, _viewableAngle, _distanceFromTarget);

        return combatStanceState;
    }

    private void HandleRotateTowardsTarget(EnemyManager _enemyManager, Vector3 _direction) {
        if (_enemyManager.IsPerformingAction) {
            _direction.y = 0;
            _direction.Normalize();

            if (_direction == Vector3.zero) _direction = _enemyManager.transform.forward;

            Quaternion _targetRotation = Quaternion.LookRotation(_direction);
            _enemyManager.transform.rotation =
                Quaternion.Slerp(transform.rotation, _targetRotation,
                    _enemyManager.RotationSpeed / Time.fixedDeltaTime);
        }
        else {
            Vector3 _relativeDirection =
                transform.InverseTransformDirection(_enemyManager.NavMeshAgent.desiredVelocity);
            Vector3 _targetVelocity = _enemyManager.Rigidbody.velocity;

            _enemyManager.NavMeshAgent.enabled = true;
            _enemyManager.NavMeshAgent.SetDestination(_enemyManager.CurrentTarget.transform.position);
            _enemyManager.Rigidbody.velocity = _targetVelocity;
            _enemyManager.transform.rotation = Quaternion.Slerp(_enemyManager.transform.rotation,
                _enemyManager.NavMeshAgent.transform.rotation,
                _enemyManager.RotationSpeed / Time.fixedDeltaTime);
        }
    }

    private void GetNewAttack(EnemyManager _enemyManager, Vector3 _targetPos, float _viewableAngle,
        float _distanceFromTarget) {
        int _maxScore = 0;

        for (int i = 0; i < EnemyAttacks.Length; i++) {
            EnemyAttackAction _enemyAttackAction = EnemyAttacks[i];

            if (_distanceFromTarget <= _enemyAttackAction.MaxDistanceNeededToAttack &&
                _distanceFromTarget >= _enemyAttackAction.MinDistanceNeededToAttack) {
                if (_viewableAngle <= _enemyAttackAction.MaxAttackAngle &&
                    _viewableAngle >= _enemyAttackAction.MinAttackAngle) {
                    _maxScore += _enemyAttackAction.AttackScore;
                }
            }
        }

        int _randomValue = Random.Range(0, _maxScore);
        int _temporaryScore = 0;

        for (int j = 0; j < EnemyAttacks.Length; j++) {
            EnemyAttackAction _enemyAttackAction = EnemyAttacks[j];

            if (_distanceFromTarget <= _enemyAttackAction.MaxDistanceNeededToAttack &&
                _distanceFromTarget >= _enemyAttackAction.MinDistanceNeededToAttack) {
                if (_viewableAngle <= _enemyAttackAction.MaxAttackAngle &&
                    _viewableAngle >= _enemyAttackAction.MinAttackAngle) {
                    if (CurrentAttack != null) return;

                    _temporaryScore += _enemyAttackAction.AttackScore;
                    if (_temporaryScore > _randomValue) CurrentAttack = _enemyAttackAction;
                }
            }
        }
    }
}
}