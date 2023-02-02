using System;
using UnityEngine;

namespace Dzajna
{
    public class EnemyManager : CharacterManager
    {
        private EnemyLocomotionManager enemyLocomotionManager;

        [Header("AI Settings")] public bool IsPerformingAction;

        // public float MinimumDetectionAngle { get; private set; }
        // public float MaximumDetectionAngle { get; private set; }
        
        // public float AggroRange { get; private set; }
        [SerializeField] float aggroRange = 10;
        public float AggroRange {
            get => aggroRange;
        }

        public float MinimumDetectionAngle = 50;
        public float MaximumDetectionAngle = -50;


        private void Awake() {
            enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
        }

        private void FixedUpdate() {
            HandleCurrentAction();
        }

        private void HandleCurrentAction() {
            if (enemyLocomotionManager.CurrentTarget == null) enemyLocomotionManager.HandleDetection();
            else enemyLocomotionManager.HandleMoveToTarget();
        }
    }
}