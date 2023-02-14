using UnityEngine;

namespace Dzajna {
public class CombatStanceState : State {
    [SerializeField] private AttackState attackState;
    [SerializeField] private PursueTargetState pursueTargetState;

    public override State Tick(EnemyManager _enemyManager, EnemyStats _enemyStats,
        EnemyAnimatorManager _enemyAnimatorManager) {
        float _distanceFromTarget =
            Vector3.Distance(_enemyManager.CurrentTarget.transform.position, _enemyManager.transform.position);

        if (_enemyManager.CurrentRecoveryTime <= 0 && _distanceFromTarget <= _enemyManager.MaxAttackRange)
            return attackState;
        else if (_distanceFromTarget > _enemyManager.MaxAttackRange)
            return pursueTargetState;
        else return this;

        // Check for attack range,
        // Potentially circle player or walk around them
        // If in attack range return attack State
        // If we are in a cooldown after attacking, return this state and continue circling player
        // if the player runs out of range return to PursueTargetState
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
}
}