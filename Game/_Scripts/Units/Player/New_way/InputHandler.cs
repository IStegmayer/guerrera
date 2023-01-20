using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dzajna
{
    public class InputHandler : MonoBehaviour
    {
        public float Horizontal;
        public float Vertical;
        public float MoveAmount;
        public float CameraX;
        public float CameraY;
        public bool SprintFlag;
        public bool RollFlag;
        public bool ComboFlag;
        public float RollInputTimer;

        public bool b_Input;
        public bool a_Input;
        public bool rb_Input;
        public bool rt_Input;
        public bool d_pad_up_Input;
        public bool d_pad_down_Input;
        public bool d_pad_left_Input;
        public bool d_pad_right_Input;

        private PlayerInputActions inputActions;
        private PlayerAttacker playerAttacker;
        private PlayerInventory playerInventory;
        private PlayerManager playerManager;

        private Vector2 movementInput;
        private Vector2 cameraInput;

        private void Awake() {
            playerAttacker = GetComponent<PlayerAttacker>();
            playerInventory = GetComponent<PlayerInventory>();
            playerManager = GetComponent<PlayerManager>();
        }

        public void OnEnable() {
            // read input and populate our fields
            if (inputActions == null) {
                inputActions = new PlayerInputActions();
                inputActions.PlayerMovement.Movement.performed += _input => movementInput = _input.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += _input => cameraInput = _input.ReadValue<Vector2>();
            }

            // enable our created inputActions 
            inputActions.Enable();
        }

        public void OnDisable() {
            // making sure to cleanup
            inputActions.Disable();
        }

        public void TickInput(float _delta) {
            HandleMoveInput(_delta);
            HandleRollInput(_delta);
            HandleAttackInput(_delta);
            HandleQuickSlotsInput();
            HandleInteractInput();
        }

        private void HandleMoveInput(float _delta) {
            Horizontal = movementInput.x;
            Vertical = movementInput.y;
            MoveAmount = Mathf.Clamp01(Mathf.Abs(Horizontal) + Mathf.Abs(Vertical));
            CameraX = cameraInput.x;
            CameraY = cameraInput.y;
        }

        private void HandleRollInput(float _delta) {
            b_Input = inputActions.PlayerActions.B.IsPressed();

            if (b_Input) {
                RollInputTimer += _delta;
                if (MoveAmount > 0) SprintFlag = true;
            }
            else {
                if (RollInputTimer > 0 && RollInputTimer < 0.5f) {
                    RollFlag = true;
                    SprintFlag = false;
                }

                RollInputTimer = 0;
            }
        }

        private void HandleAttackInput(float _delta) {
            rb_Input = inputActions.PlayerActions.RB.WasPerformedThisFrame();
            rt_Input = inputActions.PlayerActions.RT.WasPerformedThisFrame();

            // TODO: does this really belong here? also check for isInteracting
            // TODO: refactor this shit logic
            if (rb_Input) {
                if (playerManager.CanDoCombo) {
                    ComboFlag = true;
                    playerAttacker.HandleWeaponCombo(playerInventory.RightWeapon);
                    ComboFlag = false;
                }
                else if (!playerManager.IsInteracting && !playerManager.CanDoCombo) {
                    playerAttacker.HandleLightAttack(playerInventory.RightWeapon);
                }
            }

            if (rt_Input) {
                if (playerManager.CanDoCombo) {
                    ComboFlag = true;
                    playerAttacker.HandleWeaponCombo(playerInventory.RightWeapon);
                    ComboFlag = false;
                } else if (!playerManager.IsInteracting && !playerManager.CanDoCombo)
                {
                    playerAttacker.HandleHeavyAttack(playerInventory.RightWeapon);
                }
            }
        }

        private void HandleQuickSlotsInput() {
            d_pad_right_Input = inputActions.PlayerControls.DPadRight.WasPerformedThisFrame();
            d_pad_left_Input = inputActions.PlayerControls.DPadLeft.WasPerformedThisFrame();
            
            if (d_pad_right_Input) {
                playerInventory.ChangeRightWeapon();
            } else if (d_pad_left_Input) {
                playerInventory.ChangeLeftWeapon();
            }
        }

        private void HandleInteractInput() {
            a_Input = inputActions.PlayerActions.A.WasPerformedThisFrame();
            Debug.Log(a_Input);
        }
    }
}