using System;
using UnityEngine;

namespace Dzajna
{
    public class PlayerLocomotion : MonoBehaviour
    {
        private Transform cameraTransform;
        private InputHandler inputHandler;
        private PlayerManager playerManager;
        private CameraManager cameraManager;
        private PlayerStats playerStats;
        public Vector3 moveDirection;

        // these could be protected
        [HideInInspector] public Transform MyTransform;
        [HideInInspector] public PlayerAnimatorManager _playerAnimatorManager;
        public new Rigidbody Rigidbody;
        public GameObject normalCamera;

        [Header("Movement Stats")] [SerializeField]
        private float walkingSpeed = 1;

        [SerializeField] private float movementSpeed = 5;
        [SerializeField] private float sprintSpeed = 8;

        [SerializeField] private float rotationSpeed = 10;

        //TODO: improve this with terminal velocity and such
        [SerializeField] private float fallingSpeed = 160;

        [Header("Stamina Costs")] 
        [SerializeField] private float rollStaminaCost;
        [SerializeField] private float backstepStaminaCost;
        [SerializeField] private float sprintStaminaCost;
        
        [Header("Ground Detection Stats")] 
        [SerializeField] private float groundDetectionRayStartPoint = 0.5f;
        [SerializeField] private float minimumDistanceNeededToFall = 1f;
        [SerializeField] private float groundDirectionRayOffset = 0.2f;
        [SerializeField] private LayerMask ignoreForGroundCheck;
        [SerializeField] private CapsuleCollider characterCollider;
        [SerializeField] private CapsuleCollider characterCollisionBlockerCollider;
        public float InAirTimer;

        private void Awake() {
            cameraManager = FindObjectOfType<CameraManager>();
            Physics.IgnoreCollision(characterCollider, characterCollisionBlockerCollider, true);
        }

        private void Start() {
            Rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            playerManager = GetComponent<PlayerManager>();
            playerStats = GetComponent<PlayerStats>();
            cameraTransform = Camera.main.transform;
            MyTransform = transform;
            _playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            _playerAnimatorManager.Initialize();

            playerManager.IsGrounded = true;
        }

        #region Movement

        private Vector3 normalVector;
        private Vector3 targetPlayerPosition;

        public void HandleMovement(float _delta) {
            if (inputHandler.RollFlag) return;
            if (playerManager.IsInAir) return;

            moveDirection = cameraTransform.forward * inputHandler.Vertical;
            moveDirection += cameraTransform.right * inputHandler.Horizontal;
            moveDirection.Normalize();
            // moveDirection.y = Rigidbody.velocity.y;
            moveDirection.y = 0;

            float _speed;
            if (inputHandler.SprintFlag && inputHandler.MoveAmount > 0.5f) {
                _speed = sprintSpeed;
                playerManager.IsSprinting = true;
                playerStats.TakeStaminaDamage(sprintStaminaCost);
            }
            else {
                // TODO: walking isnt actually working
                _speed = inputHandler.MoveAmount < 0.5f ? walkingSpeed : movementSpeed;
                playerManager.IsSprinting = false;
            }

            moveDirection *= _speed;

            Vector3 _projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            _projectedVelocity.y = 0;
            Rigidbody.velocity = _projectedVelocity;

            if (inputHandler.LockOnFlag)
                _playerAnimatorManager.UpdateAnimatorValues(inputHandler.Vertical, inputHandler.Horizontal,
                    playerManager.IsSprinting);
            else
                _playerAnimatorManager.UpdateAnimatorValues(inputHandler.MoveAmount, 0, playerManager.IsSprinting);
        }

        public void HandleRotation(float _delta) {
            if (!_playerAnimatorManager.CanRotate) return;
            
            //TODO: this is ridiculous
            if (inputHandler.LockOnFlag && !inputHandler.SprintFlag && !inputHandler.RollFlag) {
                Vector3 _rotationDirection = moveDirection;
                _rotationDirection = cameraManager.CurrentLockOnTarget.transform.position - transform.position;
                _rotationDirection.y = 0;
                _rotationDirection.Normalize();
                Quaternion _tr = Quaternion.LookRotation(_rotationDirection);
                Quaternion _targetRotation = Quaternion.Slerp(transform.rotation, _tr, rotationSpeed * _delta);

                transform.rotation = _targetRotation;
            }
            else {
                Vector3 _targetDir = Vector3.zero;
                float _moveOverride = inputHandler.MoveAmount;

                _targetDir = cameraTransform.forward * inputHandler.Vertical;
                _targetDir += cameraTransform.right * inputHandler.Horizontal;
                _targetDir.Normalize();
                _targetDir.y = 0;

                if (_targetDir == Vector3.zero) _targetDir = MyTransform.forward;

                float _rs = rotationSpeed;

                Quaternion _transformRotation = Quaternion.LookRotation(_targetDir);
                Quaternion _targetRotation = Quaternion.Slerp(MyTransform.rotation, _transformRotation, _rs * _delta);

                MyTransform.rotation = _targetRotation;
            }
        }

        public void HandleRolling(float _delta) {
            if (_playerAnimatorManager.Anim.GetBool(_playerAnimatorManager.IsInteractingHash)) return;
            if (!inputHandler.RollFlag) return;
            if (!playerStats.CheckHasStamina()) return;

            if (inputHandler.MoveAmount > 0) {
                _playerAnimatorManager.PlayTargetAnimation(_playerAnimatorManager.RollHash, true);
                playerStats.TakeStaminaDamage(rollStaminaCost);
            }
            else {
                _playerAnimatorManager.PlayTargetAnimation(_playerAnimatorManager.BackstepHash, true);
                playerStats.TakeStaminaDamage(backstepStaminaCost);
            }
        }

        public void HandleFalling(float _delta, Vector3 _moveDirection) {
            playerManager.IsGrounded = false;
            RaycastHit hit;
            Vector3 _origin = MyTransform.position;
            _origin.y += groundDetectionRayStartPoint;

            //TODO: these hardcoded floats are pretty arbitrary 
            if (Physics.Raycast(_origin, MyTransform.forward, out hit, 0.4f)) _moveDirection = Vector3.zero;

            if (playerManager.IsInAir) {
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
                playerManager.IsGrounded = true;
                targetPlayerPosition.y = _targetPosition.y;

                if (playerManager.IsInAir) {
                    if (InAirTimer > 0.5f) {
                        Debug.Log("air time: " + InAirTimer);
                        _playerAnimatorManager.PlayTargetAnimation(_playerAnimatorManager.LandHash, true);
                    }
                    else {
                        _playerAnimatorManager.PlayTargetAnimation(_playerAnimatorManager.EmptyHash, false);
                    }

                    InAirTimer = 0;
                    playerManager.IsInAir = false;
                }
            }
            else {
                if (playerManager.IsGrounded) playerManager.IsGrounded = false;
                if (playerManager.IsInAir == false) {
                    if (playerManager.IsInteracting == false)
                        _playerAnimatorManager.PlayTargetAnimation(_playerAnimatorManager.FallHash, true);

                    Vector3 _velocity = Rigidbody.velocity;
                    _velocity.Normalize();
                    Rigidbody.velocity = _velocity * (movementSpeed / 2);
                    playerManager.IsInAir = true;
                }
            }

            if (playerManager.IsInteracting || inputHandler.MoveAmount > 0)
                MyTransform.position = Vector3.Lerp(MyTransform.position, targetPlayerPosition, Time.deltaTime / 0.1f);
            else
                MyTransform.position = targetPlayerPosition;
        }

        public void HandleJumping() {
            if (playerManager.IsInteracting) return;
            if (!inputHandler.jump_Input) return;
            if (inputHandler.MoveAmount <= 0) return;
            if (!playerStats.CheckHasStamina()) return;

            Vector3 _moveDirection = cameraTransform.forward * inputHandler.Vertical;
            _moveDirection += cameraTransform.right * inputHandler.Horizontal;

            _moveDirection.Normalize();
            _moveDirection.y = 0;
            MyTransform.rotation = Quaternion.LookRotation(_moveDirection);

            _playerAnimatorManager.PlayTargetAnimation(_playerAnimatorManager.JumpHash, true);
        }

        #endregion
    }
}