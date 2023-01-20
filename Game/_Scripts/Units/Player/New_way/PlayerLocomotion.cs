using System;
using UnityEngine;

namespace Dzajna
{
    public class PlayerLocomotion : MonoBehaviour
    {
        private Transform cameraObject;
        private InputHandler inputHandler;
        private PlayerManager PlayerManager;
        public Vector3 moveDirection;

        // these could be protected
        [HideInInspector] public Transform MyTransform;
        [HideInInspector] public AnimatorHandler AnimatorHandler;
        public new Rigidbody Rigidbody;
        public GameObject normalCamera;

        [Header("Movement Stats")] [SerializeField]
        private float walkingSpeed = 1;

        [SerializeField] private float movementSpeed = 5;
        [SerializeField] private float sprintSpeed = 8;

        [SerializeField] private float rotationSpeed = 10;

        //TODO: improve this with terminal velocity and such
        [SerializeField] private float fallingSpeed = 160;

        [Header("Ground Detection Stats")] [SerializeField]
        private float groundDetectionRayStartPoint = 0.5f;

        [SerializeField] private float minimumDistanceNeededToFall = 1f;
        [SerializeField] private float groundDirectionRayOffset = 0.2f;
        private LayerMask ignoreForGroundCheck;
        public float InAirTimer;

        private void Start() {
            Rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            PlayerManager = GetComponent<PlayerManager>();
            cameraObject = Camera.main.transform;
            MyTransform = transform;
            AnimatorHandler = GetComponent<AnimatorHandler>();
            AnimatorHandler.Initialize();

            PlayerManager.IsGrounded = true;
            //TODO: better understand this
            ignoreForGroundCheck = ~(1 << 8 | 1 << 11);
        }

        #region Movement

        private Vector3 normalVector;
        private Vector3 targetPlayerPosition;

        public void HandleMovement(float _delta) {
            if (inputHandler.RollFlag) return;
            if (PlayerManager.IsInAir) return;

            moveDirection = cameraObject.forward * inputHandler.Vertical;
            moveDirection += cameraObject.right * inputHandler.Horizontal;
            moveDirection.Normalize();
            // moveDirection.y = Rigidbody.velocity.y;
            moveDirection.y = 0;

            float _speed;
            if (inputHandler.SprintFlag && inputHandler.MoveAmount > 0.5f) {
                _speed = sprintSpeed;
                PlayerManager.IsSprinting = true;
            }
            else {
                // TODO: walking isnt actually working
                _speed = inputHandler.MoveAmount < 0.5f ? walkingSpeed : movementSpeed;
                PlayerManager.IsSprinting = false;
            }

            moveDirection *= _speed;

            Vector3 _projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            _projectedVelocity.y = 0;
            Rigidbody.velocity = _projectedVelocity;

            AnimatorHandler.UpdateAnimatorValues(inputHandler.MoveAmount, 0, PlayerManager.IsSprinting);
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

        public void HandleRolling(float _delta) {
            if (AnimatorHandler.Anim.GetBool(AnimatorHandler.IsInteractingHash)) return;
            if (!inputHandler.RollFlag) return;

            if (inputHandler.MoveAmount > 0) AnimatorHandler.PlayTargetAnimation(AnimatorHandler.RollHash, true);
            else AnimatorHandler.PlayTargetAnimation(AnimatorHandler.BackstepHash, true);
        }

        public void HandleFalling(float _delta, Vector3 _moveDirection) {
            PlayerManager.IsGrounded = false;
            RaycastHit hit;
            Vector3 _origin = MyTransform.position;
            _origin.y += groundDetectionRayStartPoint;

            //TODO: these hardcoded floats are pretty arbitrary 
            if (Physics.Raycast(_origin, MyTransform.forward, out hit, 0.4f)) _moveDirection = Vector3.zero;

            if (PlayerManager.IsInAir) {
                Rigidbody.AddForce(Vector3.down * fallingSpeed);
                Rigidbody.AddForce(transform.forward * 20f);
            }

            Vector3 _dir = _moveDirection;
            _dir.Normalize();
            _origin = _origin + _dir * groundDirectionRayOffset;

            targetPlayerPosition = MyTransform.position;

            Debug.DrawRay(_origin, Vector3.down * minimumDistanceNeededToFall, Color.red, 0.1f, false);
            if (Physics.Raycast(_origin, Vector3.down, out hit, minimumDistanceNeededToFall, ignoreForGroundCheck)) {
                normalVector = hit.normal;
                Vector3 _targetPosition = hit.point;
                PlayerManager.IsGrounded = true;
                targetPlayerPosition.y = _targetPosition.y;

                if (PlayerManager.IsInAir) {
                    if (InAirTimer > 0.5f) {
                        Debug.Log("air time: " + InAirTimer);
                        AnimatorHandler.PlayTargetAnimation(AnimatorHandler.LandHash, true);
                    }
                    else {
                        AnimatorHandler.PlayTargetAnimation(AnimatorHandler.EmptyHash, false);
                    }

                    InAirTimer = 0;
                    PlayerManager.IsInAir = false;
                }
            }
            else {
                if (PlayerManager.IsGrounded) PlayerManager.IsGrounded = false;
                if (PlayerManager.IsInAir == false) {
                    if (PlayerManager.IsInteracting == false)
                        AnimatorHandler.PlayTargetAnimation(AnimatorHandler.FallHash, true);

                    Vector3 _velocity = Rigidbody.velocity;
                    _velocity.Normalize();
                    Rigidbody.velocity = _velocity * (movementSpeed / 2);
                    PlayerManager.IsInAir = true;
                }
            }

            if (PlayerManager.IsInteracting || inputHandler.MoveAmount > 0)
                MyTransform.position = Vector3.Lerp(MyTransform.position, targetPlayerPosition, Time.deltaTime / 0.1f);
            else
                MyTransform.position = targetPlayerPosition;
        }

        #endregion
    }
}