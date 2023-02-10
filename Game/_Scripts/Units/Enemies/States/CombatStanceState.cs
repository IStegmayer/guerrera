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
}
}