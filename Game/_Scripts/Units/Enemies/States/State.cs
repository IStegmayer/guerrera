using UnityEngine;

namespace Dzajna {
public abstract class State : MonoBehaviour {
    public abstract State Tick(EnemyManager _enemyManager, EnemyStatsManager _enemyStatsManager,
        EnemyAnimatorManager _enemyAnimatorM);
}
}