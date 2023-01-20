using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Dzajna
{
    //TODO: prolly a static instance
    public class CameraManager : StaticInstance<CameraManager>
    {
        [Header("References")]
        //TODO: FIX
        public Transform playerTargetTransform;

        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Transform cameraPivotTransform;
        private Transform myTransform;
        private Vector3 cameraTransformPosition;
        private Vector3 cameraFollowVelocity = Vector3.zero;
        //TODO: only public getter
        public LayerMask ignoreLayers;

        [Header("Config")] [SerializeField] private float lookSpeed = 0.01f;
        [SerializeField] private float followSpeed = 0.01f;
        [SerializeField] private float pivotSpeed = 0.01f;
        [SerializeField] private float cameraDamp = 5f;
        [SerializeField] private float minimumPivot = -35;
        [SerializeField] private float maximumPivot = 35;
        private float targetPosition;
        private float defaultZPosition;
        private float lookAngle;
        private float pivotAngle;

        [Header("Collision")] [SerializeField] private float cameraSphereRadius = 0.2f;
        [SerializeField] private float cameraCollisionOffset = 0.2f;
        [SerializeField] private float minimumCollisionOffset = 0.2f;

        [Header("Lock on")] [SerializeField] private float maximumLockOnDistance = 30f;

        // switch this to characterManager
        private List<BasicGoblin> availableTargets = new List<BasicGoblin>();
        private Transform nearestLockOnTarget;


        protected override void Awake() {
            base.Awake();
            myTransform = transform;
            defaultZPosition = cameraTransform.localPosition.z;
            cameraTransformPosition = cameraTransform.position;
            //TODO: better undestand this
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
        }

        public void FollowTarget(float _delta) {
            Vector3 _targetPosition = Vector3.SmoothDamp(myTransform.position, playerTargetTransform.position,
                ref cameraFollowVelocity, _delta / followSpeed);
            myTransform.position = _targetPosition;

            HandleCameraCollisions(_delta);
        }

        private void HandleCameraCollisions(float _delta) {
            targetPosition = defaultZPosition;
            RaycastHit hit;
            Vector3 _direction = cameraTransform.position - cameraPivotTransform.position;
            _direction.Normalize();

            if (Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius, _direction, out hit,
                    Mathf.Abs(targetPosition), ignoreLayers)) {
                float _distance = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetPosition = -(_distance - cameraCollisionOffset);
            }

            if (Mathf.Abs(targetPosition) < minimumCollisionOffset) {
                targetPosition = -minimumCollisionOffset;
            }

            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, _delta / 0.05f);
            cameraTransformPosition.y = 0f;
            cameraTransform.localPosition = cameraTransformPosition;
        }

        public void HandleCameraRotation(float _delta, float _cameraXInput, float _cameraYInput) {
            lookAngle += (_cameraXInput * lookSpeed) / _delta;
            pivotAngle -= (_cameraYInput * pivotSpeed) / _delta;
            pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

            // set look/horizontal rotation
            Vector3 _rotation = Vector3.zero;
            _rotation.y = lookAngle;
            Quaternion _targetRotation = Quaternion.Euler(_rotation);
            myTransform.rotation = Quaternion.Slerp
                (myTransform.rotation, _targetRotation, cameraDamp * _delta);

            // set pivot/vertical rotation
            _rotation = Vector3.zero;
            _rotation.x = pivotAngle;
            _targetRotation = Quaternion.Euler(_rotation);
            cameraPivotTransform.localRotation = Quaternion.Slerp
                (cameraPivotTransform.localRotation, _targetRotation, cameraDamp * _delta);
        }

        //TODO: finish implementing this 
        public void HandleLockOn() {
            float _shortestDistance = Mathf.Infinity;

            Collider[] _overlapSphere = Physics.OverlapSphere(playerTargetTransform.position, maximumLockOnDistance);

            for (int _i = 0; _i < _overlapSphere.Length; _i++) {
                BasicGoblin _unit = _overlapSphere[_i].GetComponent<BasicGoblin>();

                if (_unit != null) {
                    Vector3 _lockTargetDirection = _unit.transform.position - playerTargetTransform.position;
                    float _distanceFromTarget =
                        Vector3.Distance(playerTargetTransform.position, _unit.transform.position);
                    float _viewableAngle = Vector3.Angle(_lockTargetDirection, cameraTransform.forward);

                    if (_unit.transform != playerTargetTransform.transform && _viewableAngle > -50 &&
                        _viewableAngle < 50 && _distanceFromTarget <= maximumLockOnDistance)
                        availableTargets.Add(_unit);
                }
            }

            for (int i = 0; i < availableTargets.Count; i++) {
                float _distanceFromTarget =
                    Vector3.Distance(playerTargetTransform.position, availableTargets[i].transform.position);
                if (_distanceFromTarget < _shortestDistance) {
                    _shortestDistance = _distanceFromTarget;
                    nearestLockOnTarget = availableTargets[i].transform;
                }
            }
        }
    }
}