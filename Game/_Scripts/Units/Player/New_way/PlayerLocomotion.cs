using System;
using UnityEngine;

namespace Dzajna
{
    public class PlayerLocomotion : MonoBehaviour
    {
        private Transform cameraObject;
        private InputHandler inputHandler;
        private PlayerManager PlayerManager;
        private Vector3 moveDirection;

        // these could be protected
        [HideInInspector] public Transform MyTransform;
        [HideInInspector] public AnimatorHandler AnimatorHandler;
        public new Rigidbody Rigidbody;
        public GameObject normalCamera;

        [Header("Movement Stats")] [SerializeField] private float movementSpeed = 5;
        [SerializeField] private float sprintSpeed = 8;
        [SerializeField] private float rotationSpeed = 10;


        private void Start() {
            Rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            PlayerManager = GetComponent<PlayerManager>();
            cameraObject = Camera.main.transform;
            MyTransform = transform;
            AnimatorHandler = GetComponent<AnimatorHandler>();
            AnimatorHandler.Initialize();
        }
        
        #region Movement
        private Vector3 normalVector;
        private Vector3 targetPosition;

        public void HandleMovement(float _delta) {
            if (inputHandler.RollFlag) return;
            
            moveDirection = cameraObject.forward * inputHandler.Vertical;
            moveDirection += cameraObject.right * inputHandler.Horizontal;
            moveDirection.Normalize();
            // moveDirection.y = Rigidbody.velocity.y;
            moveDirection.y = 0;

            float speed = movementSpeed;
            if (inputHandler.SprintFlag) {
                speed = sprintSpeed;
                PlayerManager.isSprinting = true;
            }
            moveDirection *= speed;

            Vector3 _projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            _projectedVelocity.y = 0;
            Rigidbody.velocity = _projectedVelocity;

            AnimatorHandler.UpdateAnimatorValues(inputHandler.MoveAmount, 0, PlayerManager.isSprinting);
            if (AnimatorHandler.CanRotate) HandleRotation(_delta);
        }

        private void HandleRotation(float _delta) {
            Vector3 _targetDir = Vector3.zero;
            float _moveOverride = inputHandler.MoveAmount;

            _targetDir = cameraObject.forward * inputHandler.Vertical;
            _targetDir += cameraObject.right * inputHandler.Horizontal;
            _targetDir.Normalize();
            _targetDir.y = 0;

            if (_targetDir == Vector3.zero) _targetDir = MyTransform.forward;

            float _rs = rotationSpeed;

            Quaternion _transformRotation = Quaternion.LookRotation(_targetDir);
            Quaternion _targetRotation = Quaternion.Slerp(MyTransform.rotation, _transformRotation, _rs * _delta);

            MyTransform.rotation = _targetRotation;
        }

        public void HandleRollingAndSprinting(float _delta) {
            if (AnimatorHandler.Anim.GetBool(AnimatorHandler.IsInteractingHash)) return;

            if (inputHandler.RollFlag) {
                moveDirection = cameraObject.forward * inputHandler.Vertical;
                moveDirection = cameraObject.right * inputHandler.Horizontal;
                moveDirection.Normalize();

                if (inputHandler.MoveAmount > 0) {
                    AnimatorHandler.PlayTargetAnimation(AnimatorHandler.RollHash, true);
                    moveDirection.y = 0;
                    Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                    // TODO: maybe slerp?
                    MyTransform.rotation = rollRotation;
                }
                else {
                    AnimatorHandler.PlayTargetAnimation(AnimatorHandler.BackstepHash, true);
                }
                
            }
        }
        
        #endregion
    }
}