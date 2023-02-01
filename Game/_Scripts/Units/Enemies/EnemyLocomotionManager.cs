using System;
using UnityEngine;

namespace Dzajna
{
    public class EnemyLocomotionManager : MonoBehaviour
    {
        [SerializeField] private LayerMask layerMask;
        
        private EnemyManager enemyManager;
        
        private CharacterStats currentTarget;

        private void Awake() {
            enemyManager = GetComponent<EnemyManager>();
        }

        public void HandleDetection() {
            if (currentTarget != null) return;
            
            Collider[] _colliders = Physics.OverlapSphere(transform.position, enemyManager.AggroRange, layerMask);

            for (int _i = 0; _i < _colliders.Length; _i++) {
                CharacterStats _characterStats = _colliders[_i].transform.GetComponent<CharacterStats>();

                if (_characterStats == null) continue;
                
                Vector3 _targetDir = _characterStats.transform.position - transform.position;
                float _viewableAngle = Vector3.Angle(_targetDir, transform.forward);

                if (_viewableAngle < enemyManager.MinimumDetectionAngle &&
                    _viewableAngle > enemyManager.MaximumDetectionAngle) {
                    currentTarget = _characterStats;
                }
            }
        }
    }
}