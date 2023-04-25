using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Dzajna
{
    public class CameraManager : StaticInstance<CameraManager>
    {
        [Header("References")] private InputHandler inputHandler;

        //TODO: FIX
        public Transform playerTargetTransform;
        private PlayerManager playerManager;


        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Transform cameraPivotTransform;
        private Transform myTransform;
        private Vector3 cameraTransformPosition;

        private Vector3 cameraFollowVelocity = Vector3.zero;

        //TODO: only public getter
        public LayerMask ignoreLayers;
        public LayerMask environmentLayer;

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
        [SerializeField] private float lockedPivotPosition = 2.25f;
        [SerializeField] private float unlockedPivotPosition = 1.65f;
        public CharacterManager CurrentLockOnTarget;
        private CharacterManager nearestLockOnTarget;
        private CharacterManager leftLockOnTarget;
        private CharacterManager rightLockOnTarget;
        private List<CharacterManager> availableTargets = new List<CharacterManager>();


        protected override void Awake() {
            base.Awake();
            myTransform = transform;
            defaultZPosition = cameraTransform.localPosition.z;
            cameraTransformPosition = cameraTransform.position;
        }

        private void Start() {
            inputHandler = InputHandler.Instance;
            playerManager = playerTargetTransform.GetComponent<PlayerManager>();
        }
        
        private void FixedUpdate() {
            HandleAllCameraActions();
        }

        private void HandleAllCameraActions() {
            if (playerManager == null) return;
            float delta = Time.fixedDeltaTime;
            FollowPlayer(delta);
            HandleCameraRotation(delta, InputHandler.Instance.CameraX, InputHandler.Instance.CameraY);
        }


        public void FollowPlayer(float delta) {
            Vector3 targetPos = Vector3.SmoothDamp(myTransform.position, playerTargetTransform.position,
                ref cameraFollowVelocity, cameraDamp / delta);
            myTransform.position = targetPos;

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

            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.05f);
            cameraTransformPosition.y = 0f;
            cameraTransform.localPosition = cameraTransformPosition;
        }

        public void HandleCameraRotation(float delta, float cameraXInput, float cameraYInput) {
            SetCameraHeight(delta);
            if (!inputHandler.LockOnFlag && CurrentLockOnTarget == null) {
                lookAngle += cameraXInput * lookSpeed * delta;
                pivotAngle -= cameraYInput * pivotSpeed * delta;
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
            else {
                float velocity = 0;

                Vector3 dir = CurrentLockOnTarget.transform.position - transform.position;
                dir.Normalize();
                dir.y = 0;

                //TODO: Slerp both of these?
                Quaternion targetRotation = Quaternion.LookRotation(dir);
                myTransform.rotation = targetRotation;

                dir = CurrentLockOnTarget.transform.position - cameraPivotTransform.position;
                dir.Normalize();

                targetRotation = Quaternion.LookRotation(dir);
                Vector3 eulerAngle = targetRotation.eulerAngles;
                eulerAngle.y = 0;
                cameraPivotTransform.localEulerAngles = eulerAngle;
            }
        }

        public void HandleLockOn() {
            Vector3 playerTargetPosition = playerTargetTransform.position;
            float shortestDistance = Mathf.Infinity;
            float shortestDistanceOfLeftTarget = -Mathf.Infinity;
            float shortestDistanceOfRightTarget = Mathf.Infinity;

            Collider[] colliders = Physics.OverlapSphere(playerTargetPosition, 26);

            for (int i = 0; i < colliders.Length; i++) {
                EnemyManager character = colliders[i].GetComponent<EnemyManager>();
                if (character == null) continue;

                Vector3 lockTargetDirection = character.transform.position - playerTargetPosition;
                float distanceFromTarget = Vector3.Distance(playerTargetPosition, character.transform.position);
                float viewableAngle = Vector3.Angle(lockTargetDirection, cameraTransform.forward);
                RaycastHit hit;

                if (character.transform != playerTargetTransform.transform && viewableAngle > -50 &&
                    viewableAngle < 50 && distanceFromTarget <= maximumLockOnDistance) {
                    if (Physics.Linecast(playerManager.LockOnTransform.position, character.LockOnTransform.position,
                            out hit)) {
                        Debug.DrawLine(playerManager.LockOnTransform.position, character.LockOnTransform.position);
                        if (hit.transform.gameObject.layer != environmentLayer)
                            availableTargets.Add(character);
                    }
                }
            }

            for (int j = 0; j < availableTargets.Count; j++) {
                Vector3 targetPos = availableTargets[j].transform.position;

                float distanceFromTarget =
                    Vector3.Distance(playerTargetPosition, targetPos);
                if (distanceFromTarget < shortestDistance) {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = availableTargets[j];
                }

                if (!inputHandler.LockOnFlag) return;

                Vector3 relativeEnemyPosition = playerManager.transform.InverseTransformPoint(targetPos);
                float distanceFromLeftTarget = relativeEnemyPosition.x;
                float distanceFromRightTarget = relativeEnemyPosition.x;

                if (relativeEnemyPosition.x <= 0.0f && distanceFromLeftTarget > shortestDistanceOfLeftTarget &&
                    availableTargets[j] != CurrentLockOnTarget) {
                    shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                    leftLockOnTarget = availableTargets[j];
                }
                else if (relativeEnemyPosition.x > 0.0f && distanceFromRightTarget < shortestDistanceOfRightTarget &&
                         availableTargets[j] != CurrentLockOnTarget) {
                    shortestDistanceOfRightTarget = distanceFromRightTarget;
                    rightLockOnTarget = availableTargets[j];
                }
            }
        }

        public bool SetCurrentLockOnTarget() {
            if (nearestLockOnTarget == null) return false;

            CurrentLockOnTarget = nearestLockOnTarget;
            return true;
        }

        public void SwitchLockOnTarget(bool leftInput, bool rightInput) {
            if (leftInput && leftLockOnTarget != null) CurrentLockOnTarget = leftLockOnTarget;
            if (rightInput && rightLockOnTarget != null) CurrentLockOnTarget = rightLockOnTarget;
        }

        public void ClearLockOnTargets() {
            availableTargets.Clear();
            nearestLockOnTarget = null;
            CurrentLockOnTarget = null;
            leftLockOnTarget = null;
            rightLockOnTarget = null;
        }

        private void SetCameraHeight(float delta) {
            Vector3 velocity = Vector3.zero;
            float newPivotY = CurrentLockOnTarget != null ? lockedPivotPosition : unlockedPivotPosition;
            Vector3 newPivotPosition = new Vector3(0, newPivotY);

            cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(
                cameraPivotTransform.transform.localPosition,
                newPivotPosition, ref velocity, (cameraDamp / 2) * delta);
        }

        public Quaternion GetCameraRotation() => cameraPivotTransform.rotation;
        public Transform GetCameraTransform() => cameraPivotTransform;
    }
}