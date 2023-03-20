using UnityEngine;

namespace Dzajna {
public class RotateTowardsTargetState : State {
    public override State Tick(EnemyManager _enemyManager, EnemyStats _enemyStats,
        EnemyAnimatorManager _enemyAnimatorManager) {
        _enemyAnimatorManager.Anim.SetFloat(_enemyAnimatorManager.VerticalHash, 0);
        _enemyAnimatorManager.Anim.SetFloat(_enemyAnimatorManager.HorizontalHash, 0);

        Vector3 _targetDirection = _enemyManager.CurrentTarget.transform.position - _enemyManager.transform.position;
        float _viewableAngle = Vector3.SignedAngle(_targetDirection, _enemyManager.transform.forward, Vector3.up);

        if (_viewableAngle >= 100 && _viewableAngle <= 180 && !_enemyManager.IsInteracting)
            _enemyAnimatorManager.PlayTargetAnimationWithRootRotation(_enemyAnimatorManager.TurnAroundHash, true);
        else if (_viewableAngle <= -101 && _viewableAngle >= -180 && !_enemyManager.IsInteracting)
            _enemyAnimatorManager.PlayTargetAnimationWithRootRotation(_enemyAnimatorManager.TurnAroundHash, true);
        else if (_viewableAngle <= -45 && _viewableAngle >= -100 && !_enemyManager.IsInteracting)
            _enemyAnimatorManager.PlayTargetAnimationWithRootRotation(_enemyAnimatorManager.TurnRightHash, true);
        else if (_viewableAngle >= 45 && _viewableAngle <= 100 && !_enemyManager.IsInteracting)
            _enemyAnimatorManager.PlayTargetAnimationWithRootRotation(_enemyAnimatorManager.TurnLeftHash, true);
        else {
            Debug.Log("Viewable angle: " + _viewableAngle);
        }


        return this;
    }
}
}