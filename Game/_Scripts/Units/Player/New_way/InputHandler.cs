using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

namespace Dzajna
{
    public class InputHandler : MonoBehaviour
    {
        public float Horizontal;
        public float Vertical;
        public float MoveAmount;
        public float CameraX;
        public float CameraY;
        public float RollInputTimer;

        public bool b_Input;
        public bool a_Input;
        public bool rb_Input;
        public bool rt_Input;
        public bool JumpInput;
        public bool lockOn_Input;
        public bool d_pad_up_Input;
        public bool d_pad_down_Input;
        public bool d_pad_left_Input;
        public bool d_pad_right_Input;
        public bool inventory_Input;

        public bool SprintFlag;
        public bool RollFlag;
        public bool ComboFlag;
        public bool InventoryFlag;
        public bool LockOnFlag;

        private PlayerInputActions inputActions;
        private PlayerAttacker playerAttacker;
        private PlayerInventory playerInventory;
        private PlayerManager playerManager;
        private CameraManager cameraManager;
        private UIManager uiManager;

        private Vector2 movementInput;
        private Vector2 cameraInput;

        private void Awake() {
            playerAttacker = GetComponent<PlayerAttacker>();
            playerInventory = GetComponent<PlayerInventory>();
            playerManager = GetComponent<PlayerManager>();
            // TODO: inefficient asf
            cameraManager = FindObjectOfType<CameraManager>();
            uiManager = FindObjectOfType<UIManager>();
        }

        public void OnEnable() {
            // read input and populate our fields
            if (inputActions == null) {
                inputActions = new PlayerInputActions();

                inputActions.PlayerMovement.Movement.performed += _input => movementInput = _input.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += _input => cameraInput = _input.ReadValue<Vector2>();

                // inputActions.PlayerActions.Jump.performed += HandleJumpInput;
                // inputActions.PlayerActions.A.performed += HandleInteractInput;
                inputActions.PlayerActions.RB.performed += _input => rb_Input = true;
                inputActions.PlayerActions.RT.performed += _input => rt_Input = true;
                inputActions.PlayerActions.Jump.performed += _input => JumpInput = true;
                inputActions.PlayerActions.A.performed += _input => a_Input = true;
                inputActions.PlayerActions.LockOn.performed += _input => lockOn_Input = true;

                inputActions.PlayerControls.Inventory.performed += HandleInventoryInput;
                inputActions.PlayerControls.DPadRight.performed += _input => d_pad_right_Input = true;
                inputActions.PlayerControls.DPadLeft.performed += _input => d_pad_left_Input = true;
            }

            // enable our created inputActions 
            inputActions.Enable();
        }

        public void OnDisable() {
            // making sure to cleanup
            inputActions.Disable();
        }

        public void TickInput(float _delta) {
            HandleMoveInput();
            HandleRollInput(_delta);
            HandleAttackInput();
            HandleQuickSlotsInput();
            HandleLockOnInput();
        }

        private void HandleMoveInput() {
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

        private void HandleAttackInput() {
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
                }
                else if (!playerManager.IsInteracting && !playerManager.CanDoCombo) {
                    playerAttacker.HandleHeavyAttack(playerInventory.RightWeapon);
                }
            }
        }

        private void HandleQuickSlotsInput() {
            if (d_pad_right_Input) {
                playerInventory.ChangeRightWeapon();
            }
            else if (d_pad_left_Input) {
                playerInventory.ChangeLeftWeapon();
            }
        }

        private void HandleInteractInput(CallbackContext _callbackContext) {
            a_Input = _callbackContext.action.WasPerformedThisFrame();
        }

        private void HandleJumpInput(CallbackContext _callbackContext) {
            JumpInput = _callbackContext.action.WasPerformedThisFrame();
        }

        private void HandleInventoryInput(CallbackContext _callbackContext) {
            inventory_Input = _callbackContext.action.WasPerformedThisFrame();
            if (!inventory_Input) return;

            InventoryFlag = !InventoryFlag;
            uiManager.ToggleSelectWindow(InventoryFlag);
            if (InventoryFlag) uiManager.UpdateUI();
            else uiManager.CloseAllInventoryWindows();
            uiManager.ToggleHUDWindow(!InventoryFlag);
        }

        private void HandleLockOnInput() {
            if (!lockOn_Input) return;

            if (!LockOnFlag) {
                lockOn_Input = false;
                cameraManager.HandleLockOn();
                LockOnFlag = cameraManager.SetCurrentLockOnTarget();
            }
            else {
                lockOn_Input = false;
                LockOnFlag = false;
                cameraManager.ClearLockOnTargets();
            }
        }
    }
}