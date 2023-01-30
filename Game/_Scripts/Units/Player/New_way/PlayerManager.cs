using System;
using UnityEngine;

namespace Dzajna
{
    public class PlayerManager : CharacterManager
    {
        private InputHandler inputHandler;
        private Animator anim;
        private AnimatorHandler animatorHandler;
        private CameraManager cameraManager;
        private PlayerLocomotion playerLocomotion;

        private InteractableUI interactableUI;

        [Header("Player flags")] public bool IsInteracting;
        public bool IsSprinting;
        public bool IsInAir = false;
        public bool IsGrounded;
        public bool CanDoCombo;

        private void Awake() {
            cameraManager = CameraManager.Instance;
            interactableUI = FindObjectOfType<InteractableUI>();
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
            CanDoCombo = anim.GetBool(animatorHandler.CanDoComboHash);
            IsInAir = anim.GetBool(animatorHandler.IsInAirHash);

            inputHandler.TickInput(_delta);
            CheckForInteractableObject();
            playerLocomotion.HandleMovement(_delta);
            playerLocomotion.HandleRolling(_delta);
            // TODO: WTF CALLING with its own property? 
            playerLocomotion.HandleFalling(_delta, playerLocomotion.moveDirection);
            playerLocomotion.HandleJumping();
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
            inputHandler.a_Input = false;
            inputHandler.jump_Input = false;
            inputHandler.lockOn_Input = false; 
            inputHandler.d_pad_right_Input = false;
            inputHandler.d_pad_left_Input = false;
            inputHandler.d_pad_up_Input = false;
            inputHandler.d_pad_down_Input = false;

            if (IsInAir) playerLocomotion.InAirTimer += Time.deltaTime;
        }

        public void CheckForInteractableObject() {
            RaycastHit _hit;
            Interactable _interactable;

            //TODO: not perfect, probable an overlapsphere or just ontriggerEnter/Exit would be better
            if (!Physics.SphereCast(transform.position, 1f, transform.forward, out _hit, 1,
                cameraManager.ignoreLayers)) return;
            if (!_hit.transform.TryGetComponent(out _interactable)) {
                if (interactableUI.InteractableUIGameObject != null)
                    interactableUI.InteractableUIGameObject.SetActive(false);
                return;
            }

            string _interactableText = _interactable.InteractableText;
            interactableUI.InteractableText.text = _interactableText;
            interactableUI.InteractableUIGameObject.SetActive(true);

            if (inputHandler.a_Input) {
                _interactable.Interact(this);
            }
        }
    }
}