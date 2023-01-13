using System;
using UnityEngine;

namespace Dzajna
{
    public class PlayerManager : MonoBehaviour
    {
        private InputHandler inputHandler;
        private Animator anim;
        private CameraManager cameraManager;
        private PlayerLocomotion playerLocomotion;
        
        [Header("Player flags")]
        public bool IsInteracting;
        public bool isSprinting;

        private void Awake() {
            cameraManager = CameraManager.Instance;
        }
        
        private void Start() {
            inputHandler = GetComponent<InputHandler>();
            anim = GetComponent<Animator>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
        }

        private void Update() {
            float _delta = Time.deltaTime;
            //TODO: hash this shit
            IsInteracting = anim.GetBool("isInteracting");

            isSprinting = inputHandler.b_Input;
            inputHandler.TickInput(_delta);
            playerLocomotion.HandleMovement(_delta);
            playerLocomotion.HandleRollingAndSprinting(_delta);
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
            isSprinting = false;
        }
    }
}