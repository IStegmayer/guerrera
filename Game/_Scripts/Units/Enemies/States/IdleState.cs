using UnityEngine;

namespace Dzajna {
public class IdleState : State {
    [SerializeField] private PursueTargetState pursueTargetState;
    [SerializeField] private LayerMask layerMask;

    public override State Tick(EnemyManager _enemyManager, EnemyStats _enemyStats,
        EnemyAnimatorManager _enemyAnimatorM) {
        if (_enemyManager.IsInteracting) return this;
        
        // Look for a potential target
        Collider[] _colliders =
            Physics.OverlapSphere(_enemyManager.transform.position, _enemyManager.AggroRange, layerMask);
        
        for (int _i = 0; _i < _colliders.Length; _i++) {
            CharacterStats _characterStats = _colliders[_i].transform.GetComponent<CharacterStats>();
        
            if (_characterStats != null) {
                Vector3 _targetDir = _characterStats.transform.position - _enemyManager.transform.position;
                float _viewableAngle = Vector3.Angle(_targetDir, _enemyManager.transform.forward);
        
                if (_viewableAngle < _enemyManager.MinDetectionAngle &&
                    _viewableAngle > _enemyManager.MaxDetectionAngle) {
                    _enemyManager.CurrentTarget = _characterStats;
                }
            }
        }

        if (_enemyManager.CurrentTarget != null) return pursueTargetState;
        else return this;
    }
}
}