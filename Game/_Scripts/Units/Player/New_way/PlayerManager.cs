using System;
using UnityEngine;

namespace Dzajna
{
    public class PlayerManager : MonoBehaviour
    {
        private InputHandler inputHandler;
        private Animator anim;
        private AnimatorHandler animatorHandler;
        private CameraManager cameraManager;
        private PlayerLocomotion playerLocomotion;
        
        [Header("Player flags")]
        public bool IsInteracting;
        public bool IsSprinting;
        public bool IsInAir;
        public bool IsGrounded;

        private void Awake() {
            cameraManager = CameraManager.Instance;
        }
        
        private void Start() {
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponent<AnimatorHandler>();
            anim = GetComponent<Animator>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
        }

        private void Update() {
            float _delta = Time.deltaTime;
            IsInteracting = anim.GetBool(animatorHandler.IsInteractingHash);
            
            inputHandler.TickInput(_delta);
            playerLocomotion.HandleMovement(_delta);
            playerLocomotion.HandleRolling(_delta);
            // TODO: WTF CALLING with its own property? 
            playerLocomotion.HandleFalling(_delta, playerLocomotion.moveDirection);
        }
        
        private void FixedUpdate() {
            float _delta = Time.fixedDeltaTime;
            if (cameraManager != null) {
                cameraManager.FollowTarget(_delta);
                cameraManager.HandleCameraRotation(_delta, inputHandler.CameraX, inputHandler.CameraY);
            }
        }

        private void LateUpdate() {
            //reset flags
            inputHandler.RollFlag = false;
            inputHandler.SprintFlag = false;
            inputHandler.rb_Input = false;
            inputHandler.rt_Input = false;
            if (IsInAir) playerLocomotion.InAirTimer += Time.deltaTime;
        }
    }
}