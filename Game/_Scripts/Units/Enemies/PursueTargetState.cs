using UnityEngine;

namespace Dzajna {
public class PursueTargetState : State {
    [SerializeField] private CombatStanceState combatStanceState;

    public override State Tick(EnemyManager _enemyManager, EnemyStats _enemyStats,
        EnemyAnimatorManager _enemyAnimatorManager) {
        if (_enemyManager.IsPerformingAction) return this;
        // Chase the target

        Vector3 _targetDir = _enemyManager.CurrentTarget.transform.position - transform.position;
        _enemyManager.DistanceFromTarget =
            Vector3.Distance(_enemyManager.CurrentTarget.transform.position, transform.position);
        float _viewableAngle = Vector3.Angle(_targetDir, transform.forward);

        if (_enemyManager.DistanceFromTarget > _enemyManager.MaxAttackRange)
            _enemyAnimatorManager.Anim.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
        else if (_enemyManager.DistanceFromTarget <= _enemyManager.MaxAttackRange)
            _enemyAnimatorManager.Anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);


        HandleRotateTowardsTarget(_enemyManager);
        _enemyManager.NavMeshAgent.transform.localPosition = Vector3.zero;
        _enemyManager.NavMeshAgent.transform.localRotation = Quaternion.identity;
        // If within attack range, switch to combat stance state
        // If target is out of attack range, return this state and continue giving chase
        if (_enemyManager.DistanceFromTarget <= _enemyManager.MaxAttackRange) return combatStanceState;
        else return this;
    }

    private void HandleRotateTowardsTarget(EnemyManager _enemyManager) {
        if (_enemyManager.IsPerformingAction) {
            Vector3 _direction = _enemyManager.CurrentTarget.transform.position - transform.position;
            _direction.y = 0;
            _direction.Normalize();

            if (_direction == Vector3.zero) _direction = transform.forward;

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
            _enemyManager.transform.rotation = Quaternion.Slerp(transform.rotation, _enemyManager.NavMeshAgent.transform.rotation,
                _enemyManager.RotationSpeed / Time.fixedDeltaTime);
        }
    }
}
}