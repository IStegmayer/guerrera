using System;
using UnityEngine;

namespace Dzajna
{
    public class EnemyManager : CharacterManager
    {
        private EnemyLocomotionManager enemyLocomotionManager;
        
        [Header("AI Settings")]
        private bool isPerformingAction;
        //
        // public float AggroRange { get; private set; }
        // public float MinimumDetectionAngle { get; private set; }
        // public float MaximumDetectionAngle { get; private set; }
        public float AggroRange = 10;
        public float MinimumDetectionAngle = 50;
        public float MaximumDetectionAngle = -50;

        
        private void Awake() {
            enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
        }

        private void Update() {
            HandleCurrentAction();
        }

        private void HandleCurrentAction() {
            enemyLocomotionManager.HandleDetection();
        }
    }
}