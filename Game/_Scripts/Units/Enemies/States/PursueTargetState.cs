using UnityEngine;

namespace Dzajna {
public class PursueTargetState : State {
    [SerializeField] private CombatStanceState combatStanceState;
    [SerializeField] private RotateTowardsTargetState rotateTowardsTargetState;

    public override State Tick(EnemyManager _enemyManager, EnemyStatsManager _enemyStatsManager,
        EnemyAnimatorManager _enemyAnimatorM) {
        if (_enemyManager.IsInteracting) return this;

        Vector3 _targetDir = _enemyManager.CurrentTarget.transform.position - _enemyManager.transform.position;
        HandleRotateTowardsTarget(_enemyManager, _targetDir);
        
        if (_enemyManager.IsPerformingAction) {
            _enemyAnimatorM.Anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            return this;
        }
        
        // Chase the target
        float _distanceFromTarget =
            Vector3.Distance(_enemyManager.CurrentTarget.transform.position, _enemyManager.transform.position);
        float _viewableAngle = Vector3.SignedAngle(_targetDir, _enemyManager.transform.forward, Vector3.up);
        if (_distanceFromTarget > _enemyManager.MaxAttackRange)
            _enemyAnimatorM.Anim.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
        

        // If within attack range, switch to combat stance state
        // If target is out of attack range, return this state and continue giving chase
        if (_distanceFromTarget <= _enemyManager.MaxAttackRange) return combatStanceState;
        else return this;
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
            _enemyManager.transform.rotation = Quaternion.Slerp(_enemyManager.transform.rotation, _enemyManager.NavMeshAgent.transform.rotation,
                _enemyManager.RotationSpeed / Time.fixedDeltaTime);
        }
    }
}
}