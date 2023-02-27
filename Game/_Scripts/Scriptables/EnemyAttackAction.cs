using UnityEngine;

namespace Dzajna
{
    [CreateAssetMenu(menuName = "AI/Enemy Actions/Attack Action")]
    public class EnemyAttackAction : EnemyAction {
        public bool canCombo;
        public EnemyAttackAction comboAction;
        
        public int AttackScore = 3;
        public float RecoveryTime = 2;

        public float MaxAttackAngle = 35;
        public float MinAttackAngle = -35;

        public float MinDistanceNeededToAttack = 0;
        public float MaxDistanceNeededToAttack = 3;
    }
}