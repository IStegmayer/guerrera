using UnityEngine;

namespace Dzajna {
public class CombatStanceState : State {
    [SerializeField] private AttackState attackState;
    [SerializeField] private PursueTargetState pursueTargetState;
    public EnemyAttackAction[] EnemyAttacks;

    protected bool randomDestinationSet;
    protected float verticalMovementValue;
    protected float horizontalMovementValue;

    public override State Tick(EnemyManager _enemyManager, EnemyStats _enemyStats,
        EnemyAnimatorManager _enemyAnimatorM) {
        if (_enemyManager.IsInteracting) {
            _enemyAnimatorM.Anim.SetFloat(_enemyAnimatorM.VerticalHash, 0);
            _enemyAnimatorM.Anim.SetFloat(_enemyAnimatorM.HorizontalHash, 0);
            return this;
        }
        
        attackState.SetHasPerformedAttack(false);

        Vector3 _targetPos = _enemyManager.CurrentTarget.transform.position;
        Vector3 _targetsDirection = _targetPos - _enemyManager.transform.position;
        float _distanceFromTarget = Vector3.Distance(_targetPos, _enemyManager.transform.position);
        float _viewableAngle = Vector3.Angle(_targetsDirection, _enemyManager.transform.forward);
        
        _enemyAnimatorM.Anim.SetFloat(_enemyAnimatorM.VerticalHash, verticalMovementValue, 0.2f, Time.deltaTime);
        _enemyAnimatorM.Anim.SetFloat(_enemyAnimatorM.HorizontalHash, horizontalMovementValue, 0.2f, Time.deltaTime);

        if (_distanceFromTarget > _enemyManager.MaxAttackRange)
            return pursueTargetState;

        if (!randomDestinationSet) {
            randomDestinationSet = true;
            DecideCirclingAction(_enemyAnimatorM);
        }

        HandleRotateTowardsTarget(_enemyManager, _targetsDirection);

        if (_enemyManager.CurrentRecoveryTime <= 0 && attackState.CurrentAttack != null) {
            randomDestinationSet = false;
            return attackState;
        }
        
        GetNewAttack(_enemyManager, _targetPos, _viewableAngle, _distanceFromTarget);
        return this;
    }
    
    protected virtual void GetNewAttack(EnemyManager _enemyManager, Vector3 _targetPos, float _viewableAngle,
        float _distanceFromTarget) {
        int _maxScore = 0;
        EnemyAttackAction[] _attacks = GetEligibleAttacks(); 

        for (int i = 0; i < _attacks.Length; i++) {
            EnemyAttackAction _enemyAttackAction = _attacks[i];

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

        for (int j = 0; j < _attacks.Length; j++) {
            EnemyAttackAction _enemyAttackAction = _attacks[j];

            if (_distanceFromTarget <= _enemyAttackAction.MaxDistanceNeededToAttack &&
                _distanceFromTarget >= _enemyAttackAction.MinDistanceNeededToAttack) {
                if (_viewableAngle <= _enemyAttackAction.MaxAttackAngle &&
                    _viewableAngle >= _enemyAttackAction.MinAttackAngle) {
                    if (attackState.CurrentAttack != null) return;

                    _temporaryScore += _enemyAttackAction.AttackScore;
                    if (_temporaryScore > _randomValue) attackState.CurrentAttack = _enemyAttackAction;
                }
            }
        }
    }

    protected virtual void HandleRotateTowardsTarget(EnemyManager _enemyManager, Vector3 _direction) {
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

    protected virtual void DecideCirclingAction(EnemyAnimatorManager _enemyAnimatorManager) {
        WalkAroundTarget(_enemyAnimatorManager);
        // Circle with only forward vertical movement
        // Circle with running 
        // Circle with walking only
        // Don't circle
    }

    protected virtual void WalkAroundTarget(EnemyAnimatorManager _enemyAnimatorM) {
        float _walkSpeed = 0.5f;
        float _runSpeed = 1f;
        verticalMovementValue = _walkSpeed; // To include backwards, values (-1,1)
        horizontalMovementValue = Random.Range(-1, 1);

        if (horizontalMovementValue <= 1 && horizontalMovementValue >= 0) horizontalMovementValue = 0.5f;
        else if (horizontalMovementValue >= -1 && horizontalMovementValue < 0) horizontalMovementValue = -0.5f;
    }

    protected virtual EnemyAttackAction[] GetEligibleAttacks() {
        return EnemyAttacks;
    }
}
}