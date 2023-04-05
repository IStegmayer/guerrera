using UnityEngine;

namespace Dzajna {
public class RotateTowardsTargetState : State {
    [SerializeField] private CombatStanceState combatStanceState;
    
    public override State Tick(EnemyManager _enemyManager, EnemyStatsManager _enemyStatsManager,
        EnemyAnimatorManager _enemyAnimatorM) {
        _enemyAnimatorM.Anim.SetFloat(_enemyAnimatorM.VerticalHash, 0);
        _enemyAnimatorM.Anim.SetFloat(_enemyAnimatorM.HorizontalHash, 0);

        Vector3 _targetDirection = _enemyManager.CurrentTarget.transform.position - _enemyManager.transform.position;
        float _viewableAngle = Vector3.SignedAngle(_targetDirection, _enemyManager.transform.forward, Vector3.up);

        if (_enemyManager.IsInteracting) return this; //Wait until we stop interacting 

        if (_viewableAngle >= 100 && _viewableAngle <= 180 && !_enemyManager.IsInteracting)
            _enemyAnimatorM.PlayTargetAnimationWithRootRotation(_enemyAnimatorM.TurnAroundHash, true);
        else if (_viewableAngle <= -101 && _viewableAngle >= -180 && !_enemyManager.IsInteracting)
            _enemyAnimatorM.PlayTargetAnimationWithRootRotation(_enemyAnimatorM.TurnAroundHash, true);
        else if (_viewableAngle <= -45 && _viewableAngle >= -100 && !_enemyManager.IsInteracting)
            _enemyAnimatorM.PlayTargetAnimationWithRootRotation(_enemyAnimatorM.TurnRightHash, true);
        else if (_viewableAngle >= 45 && _viewableAngle <= 100 && !_enemyManager.IsInteracting)
            _enemyAnimatorM.PlayTargetAnimationWithRootRotation(_enemyAnimatorM.TurnLeftHash, true);
        else {
            Debug.Log("Viewable angle: " + _viewableAngle);
        }


        return combatStanceState;
    }
}
}