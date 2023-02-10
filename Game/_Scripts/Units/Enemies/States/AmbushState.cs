using UnityEngine;

namespace Dzajna {
public class AmbushState : State {
    public bool IsSleeping;
    [SerializeField] private float detectionRadius = 20;
    [SerializeField] private string sleepAnimation;
    [SerializeField] private string wakeAnimation;
    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] PursueTargetState pursueTargetState;

    public override State Tick(EnemyManager _enemyManager, EnemyStats _enemyStats,
        EnemyAnimatorManager _enemyAnimatorManager) {
        if (IsSleeping && !_enemyManager.IsInteracting) {
            _enemyAnimatorManager.PlayTargetAnimation(sleepAnimation, true);
        }

        HandleTargetDetection(_enemyManager, _enemyAnimatorManager);
        if (_enemyManager.CurrentTarget != null) return pursueTargetState;
        else return this;
    }

    private void HandleTargetDetection(EnemyManager _enemyManager, EnemyAnimatorManager _enemyAnimatorManager) {
        Collider[] _colliders =
            Physics.OverlapSphere(_enemyManager.transform.position, detectionRadius, detectionLayer);

        for (int i = 0; i < _colliders.Length; i++) {
            CharacterStats _characterStats = _colliders[i].transform.GetComponent<CharacterStats>();
            if (_characterStats == null) continue;

            Vector3 _targetsDirection = _characterStats.transform.position - _enemyManager.transform.position;
            float _viewableAngle = Vector3.Angle(_targetsDirection, _enemyManager.transform.forward);

            if (_viewableAngle < _enemyManager.MinDetectionAngle && _viewableAngle > _enemyManager.MaxDetectionAngle) {
                _enemyManager.CurrentTarget = _characterStats;
                IsSleeping = false;
                _enemyAnimatorManager.PlayTargetAnimation(wakeAnimation, true);
            }
        }
    }
}
}