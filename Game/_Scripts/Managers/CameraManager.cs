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
        private LayerMask ignoreLayers;

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

        public void FollowTarget(float delta) {
            Vector3 _targetPosition = Vector3.SmoothDamp(myTransform.position, playerTargetTransform.position,
                ref cameraFollowVelocity, delta / followSpeed);
            myTransform.position = _targetPosition;

            HandleCameraCollisions(delta);
        }

        private void HandleCameraCollisions(float delta) {
            targetPosition = defaultZPosition;
            RaycastHit hit;
            Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
            direction.Normalize();

            if (Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius, direction, out hit,
                    Mathf.Abs(targetPosition), ignoreLayers)) {
                float distance = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetPosition = -(distance - cameraCollisionOffset);
            }

            if (Mathf.Abs(targetPosition) < minimumCollisionOffset) {
                targetPosition = -minimumCollisionOffset;
            }

            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f);
            cameraTransformPosition.y = 0f;
            cameraTransform.localPosition = cameraTransformPosition;
        }

        public void HandleCameraRotation(float delta, float cameraXInput, float cameraYInput) {
            lookAngle += (cameraXInput * lookSpeed) / delta;
            pivotAngle -= (cameraYInput * pivotSpeed) / delta;
            pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

            // set look/horizontal rotation
            Vector3 rotation = Vector3.zero;
            rotation.y = lookAngle;
            Quaternion targetRotation = Quaternion.Euler(rotation);
            myTransform.rotation = Quaternion.Slerp
                (myTransform.rotation, targetRotation, cameraDamp * delta);

            // set pivot/vertical rotation
            rotation = Vector3.zero;
            rotation.x = pivotAngle;
            targetRotation = Quaternion.Euler(rotation);
            cameraPivotTransform.localRotation = Quaternion.Slerp
                (cameraPivotTransform.localRotation, targetRotation, cameraDamp * delta);
        }

        //TODO: finish implementing this 
        public void HandleLockOn() {
            float shortestDistance = Mathf.Infinity;

            Collider[] colliders = Physics.OverlapSphere(playerTargetTransform.position, maximumLockOnDistance);

            for (int i = 0; i < colliders.Length; i++) {
                BasicGoblin unit = colliders[i].GetComponent<BasicGoblin>();

                if (unit != null) {
                    Vector3 lockTargetDirection = unit.transform.position - playerTargetTransform.position;
                    float distanceFromTarget =
                        Vector3.Distance(playerTargetTransform.position, unit.transform.position);
                    float viewableAngle = Vector3.Angle(lockTargetDirection, cameraTransform.forward);

                    if (unit.transform != playerTargetTransform.transform && viewableAngle > -50 &&
                        viewableAngle < 50 && distanceFromTarget <= maximumLockOnDistance)
                        availableTargets.Add(unit);
                }
            }

            for (int i = 0; i < availableTargets.Count; i++) {
                float distanceFromTarget =
                    Vector3.Distance(playerTargetTransform.position, availableTargets[i].transform.position);
                if (distanceFromTarget < shortestDistance) {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = availableTargets[i].transform;
                }
            }
        }
    }
}