using System;
using UnityEngine;
using UnityEngine.AI;

namespace Dzajna
{
    public class EnemyLocomotionManager : MonoBehaviour
    {
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float rotationSpeed = 100f;
        [SerializeField] private float stoppingDistance = 1f;

        private EnemyManager enemyManager;
        private EnemyAnimatorManager enemyAnimatorManager;
        private NavMeshAgent navMeshAgent;
        private Rigidbody rigidbody;

        public CharacterStats CurrentTarget;

        private void Awake() {
            enemyManager = GetComponent<EnemyManager>();
            enemyAnimatorManager = GetComponent<EnemyAnimatorManager>();
            navMeshAgent = GetComponentInChildren<NavMeshAgent>();
            rigidbody = GetComponent<Rigidbody>();
        }

        private void Start() {
            navMeshAgent.enabled = false;
            rigidbody.isKinematic = false;
        }

        public void HandleDetection() {
            Collider[] _colliders = Physics.OverlapSphere(transform.position, enemyManager.AggroRange, layerMask);

            for (int _i = 0; _i < _colliders.Length; _i++) {
                CharacterStats _characterStats = _colliders[_i].transform.GetComponent<CharacterStats>();

                if (_characterStats != null) {
                    Vector3 _targetDir = _characterStats.transform.position - transform.position;
                    float _viewableAngle = Vector3.Angle(_targetDir, transform.forward);

                    if (_viewableAngle < enemyManager.MinimumDetectionAngle &&
                        _viewableAngle > enemyManager.MaximumDetectionAngle) {
                        CurrentTarget = _characterStats;
                    }
                }
            }
        }

        public void HandleMoveToTarget() {
            Vector3 _targetDir = CurrentTarget.transform.position - transform.position;
            float _viewableAngle = Vector3.Angle(_targetDir, transform.forward);
            float _distanceFromTarget = Vector3.Distance(CurrentTarget.transform.position, transform.position);

            if (enemyManager.IsPerformingAction) {
                enemyAnimatorManager.Anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                navMeshAgent.enabled = false;
            }
            else {
                if (_distanceFromTarget > stoppingDistance)
                    enemyAnimatorManager.Anim.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
                else if (_distanceFromTarget <= stoppingDistance)
                    enemyAnimatorManager.Anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            }

            HandleRotateTowardsTarget();

            transform.position =
                new Vector3(transform.position.x, navMeshAgent.transform.position.y, transform.position.z);
            navMeshAgent.transform.localPosition = Vector3.zero;
            navMeshAgent.transform.localRotation = Quaternion.identity;
        }

        private void HandleRotateTowardsTarget() {
            if (enemyManager.IsPerformingAction) {
                Vector3 _direction = CurrentTarget.transform.position - transform.position;
                _direction.y = 0;
                _direction.Normalize();

                if (_direction == Vector3.zero) _direction = transform.forward;

                Quaternion _targetRotation = Quaternion.LookRotation(_direction);
                transform.rotation =
                    Quaternion.Slerp(transform.rotation, _targetRotation, rotationSpeed * Time.fixedDeltaTime);
            }
            else {
                Vector3 _relativeDirection = transform.InverseTransformDirection(navMeshAgent.desiredVelocity);
                Vector3 _targetVelocity = rigidbody.velocity;

                navMeshAgent.enabled = true;
                navMeshAgent.SetDestination(CurrentTarget.transform.position);
                rigidbody.velocity = _targetVelocity;
                transform.rotation = Quaternion.Slerp(transform.rotation, navMeshAgent.transform.rotation,
                    rotationSpeed * Time.fixedDeltaTime);
            }
        }
    }
}