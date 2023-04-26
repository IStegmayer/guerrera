using System;
using UnityEngine;

namespace Dzajna
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        private Transform cameraTransform;
        private InputHandler inputHandler;
        private PlayerManager player;
        private PlayerCamera playerCamera;
        private PlayerStats playerStats;

        // TODO: what?
        [HideInInspector] public Transform MyTransform;
        [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;

        [Header("Movement Stats")] [SerializeField]
        private float walkingSpeed = 1;

        [SerializeField] private float movementSpeed = 5;
        [SerializeField] private float sprintSpeed = 8;

        [SerializeField] private float rotationSpeed = 10;

        [Header("Stamina Costs")] 
        [SerializeField] private float rollStaminaCost;
        [SerializeField] private float backstepStaminaCost;
        [SerializeField] private float sprintStaminaCost;
        

        protected override void Start() {
            base.Start();
            playerCamera = PlayerCamera.Instance;
            inputHandler = InputHandler.Instance;
            player = GetComponent<PlayerManager>();
            playerStats = GetComponent<PlayerStats>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            cameraTransform = Camera.main.transform;
            MyTransform = transform;
        }

        protected override void Update() {
            base.Update();
        }

        #region Movement

        private Vector3 targetPlayerPosition;

        public void HandleGroundMovement(float delta) {
            if (inputHandler.RollFlag) return;
            if (player.IsInteracting) return;
            if (!player.IsGrounded) return;
            float moveSpeed = movementSpeed;

            if (player.IsSprinting) {
                moveSpeed = sprintSpeed;
                playerStats.TakeStaminaDamage(sprintStaminaCost * delta);
            }
            if (inputHandler.MoveAmount <= 0.5f) moveSpeed = walkingSpeed; 

            moveDirection = playerCamera.transform.forward * inputHandler.Vertical;
            moveDirection += playerCamera.transform.right * inputHandler.Horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0;
            
            characterController.Move( moveSpeed * moveDirection * delta);
            
            if (inputHandler.LockOnFlag)
                playerAnimatorManager.UpdateAnimatorValues(inputHandler.Vertical, inputHandler.Horizontal,
                    player.IsSprinting);
            else
                playerAnimatorManager.UpdateAnimatorValues(inputHandler.MoveAmount, 0, player.IsSprinting);
        }

        public void HandleRotation(float delta) {
            if (!playerAnimatorManager.CanRotate) return;
            
            if (inputHandler.LockOnFlag && !player.IsSprinting) {
                Vector3 rotationDirection = moveDirection;
                rotationDirection = playerCamera.CurrentLockOnTarget.transform.position - transform.position;
                rotationDirection.y = 0;
                rotationDirection.Normalize();
                Quaternion tr = Quaternion.LookRotation(rotationDirection);
                Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * delta);

                transform.rotation = targetRotation;
            }
            else {
                Vector3 targetDir = Vector3.zero;
                float moveOverride = inputHandler.MoveAmount;

                targetDir = cameraTransform.forward * inputHandler.Vertical;
                targetDir += cameraTransform.right * inputHandler.Horizontal;
                targetDir.Normalize();
                targetDir.y = 0;

                if (targetDir == Vector3.zero) targetDir = MyTransform.forward;

                float rs = rotationSpeed;

                Quaternion transformRotation = Quaternion.LookRotation(targetDir);
                Quaternion targetRotation = Quaternion.Slerp(MyTransform.rotation, transformRotation, rs * delta);

                MyTransform.rotation = targetRotation;
            }
        }

        public void HandleRolling(float delta) {
            if (playerAnimatorManager.Anim.GetBool(playerAnimatorManager.IsInteractingHash)) return;
            if (!inputHandler.RollFlag) return;
            if (!playerStats.CheckHasStamina()) return;

            if (inputHandler.MoveAmount > 0) {
                playerAnimatorManager.PlayTargetAnimation(playerAnimatorManager.RollHash, true);
                playerStats.TakeStaminaDamage(rollStaminaCost);
            }
            else {
                playerAnimatorManager.PlayTargetAnimation(playerAnimatorManager.BackstepHash, true);
                playerStats.TakeStaminaDamage(backstepStaminaCost);
            }
        }

        public void HandleJumping() {
            if (player.IsInteracting) return;
            if (!inputHandler.jump_Input) return;
            if (inputHandler.MoveAmount <= 0) return;
            if (!playerStats.CheckHasStamina()) return;

            Vector3 moveDirection = cameraTransform.forward * inputHandler.Vertical;
            moveDirection += cameraTransform.right * inputHandler.Horizontal;

            moveDirection.Normalize();
            MyTransform.rotation = Quaternion.LookRotation(moveDirection);

            playerAnimatorManager.PlayTargetAnimation(playerAnimatorManager.JumpHash, true);
        }

        #endregion

        public void DisableColliders() => characterController.enabled = false;
        public void EnableColliders() => characterController.enabled = true;
    }

}