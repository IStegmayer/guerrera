using System;
using UnityEngine;

namespace Dzajna
{
    //TODO: prolly a static instance
    public class CameraManager : StaticInstance<CameraManager>
    {
        [Header("References")]
        //TODO: FIX
        public Transform targetTransform;

        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Transform cameraPivotTransform;
        private Transform myTransform;
        private Transform cameraTransformPosition;
        private Vector3 cameraFollowVelocity = Vector3.zero;
        private LayerMask ignoreLayers;

        [Header("Config")] [SerializeField] private float lookSpeed = 0.01f;
        [SerializeField] private float followSpeed = 0.01f;
        [SerializeField] private float pivotSpeed = 0.01f;
        [SerializeField] private float cameraDamp = 0.3f;
        [SerializeField] private float minimumPivot = -35;
        [SerializeField] private float maximumPivot = 35;
        private float defaultPosition;
        private float lookAngle;
        private float pivotAngle;

        protected override void Awake() {
            base.Awake();
            myTransform = transform;
            defaultPosition = cameraTransform.localPosition.z;
            //TODO: better undestand this
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
        }

        public void FollowTarget(float delta) {
            Vector3 targetPostion = Vector3.SmoothDamp(myTransform.position, targetTransform.position,
                ref cameraFollowVelocity, delta / followSpeed);
            myTransform.position = targetPostion;
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
    }
}