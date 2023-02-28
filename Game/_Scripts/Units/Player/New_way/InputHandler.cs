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
        public bool lt_Input;
        public bool lb_Input;
        public bool critical_Input;
        public bool jump_Input;
        public bool twoHand_Input;
        public bool inventory_Input;
        public bool lockOn_Input;
        public bool lockOn_left_Input;
        public bool lockOn_right_Input;
        
        public bool d_pad_up_Input;
        public bool d_pad_down_Input;
        public bool d_pad_left_Input;
        public bool d_pad_right_Input;

        public bool SprintFlag;
        public bool RollFlag;
        public bool ComboFlag;
        public bool InventoryFlag;
        public bool LockOnFlag;
        public bool TwoHandFlag;

        private PlayerAnimatorManager playerAnimatorManager;
        private PlayerInputActions inputActions;
        private PlayerAttacker playerAttacker;
        private PlayerInventory playerInventory;
        private PlayerManager playerManager;
        private PlayerStats playerStats;
        private PlayerEquipmentManager playerEquipmentManager;
        private CameraManager cameraManager;
        private UIManager uiManager;
        private WeaponSlotManager weaponSlotManager;

        private Vector2 movementInput;
        private Vector2 cameraInput;

        private void Awake() {
            playerAttacker = GetComponent<PlayerAttacker>();
            playerInventory = GetComponent<PlayerInventory>();
            playerManager = GetComponent<PlayerManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerStats = GetComponent<PlayerStats>();
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
            playerEquipmentManager = GetComponentInChildren<PlayerEquipmentManager>();
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
                inputActions.PlayerActions.RB.performed += _ => rb_Input = true;
                inputActions.PlayerActions.RT.performed += _ => rt_Input = true;
                inputActions.PlayerActions.LT.performed += _ => lt_Input = true;
                inputActions.PlayerActions.LB.performed += _ => lb_Input = true;
                inputActions.PlayerActions.LB.canceled += _ => lb_Input = false;
                inputActions.PlayerActions.Critical.performed += _ => critical_Input = true;
                inputActions.PlayerActions.Jump.performed += _ => jump_Input = true;
                inputActions.PlayerActions.A.performed += _ => a_Input = true;
                inputActions.PlayerActions.B.performed += _ => b_Input = true;
                inputActions.PlayerActions.B.canceled += _ => b_Input = false;
                inputActions.PlayerActions.LockOn.performed += _ => lockOn_Input = true;
                inputActions.PlayerActions.LockOnTargetLeft.performed += _ => lockOn_left_Input = true;
                inputActions.PlayerActions.LockOnTargetRight.performed += _ => lockOn_right_Input = true;
                inputActions.PlayerActions.TwoHand.performed += _ => twoHand_Input = true;

                inputActions.PlayerControls.Inventory.performed += HandleInventoryInput;
                inputActions.PlayerControls.DPadRight.performed += _ => d_pad_right_Input = true;
                inputActions.PlayerControls.DPadLeft.performed += _ => d_pad_left_Input = true;
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
            HandleCombatInput();
            HandleQuickSlotsInput();
            HandleLockOnInput();
            HandleLockOnSwitch();
            HandleTwoHandInput();
            HandleCriticalInput();
        }

        private void HandleMoveInput() {
            Horizontal = movementInput.x;
            Vertical = movementInput.y;
            MoveAmount = Mathf.Clamp01(Mathf.Abs(Horizontal) + Mathf.Abs(Vertical));
            CameraX = cameraInput.x;
            CameraY = cameraInput.y;
        }

        private void HandleRollInput(float _delta) {
            if (b_Input) {
                RollInputTimer += _delta;
                bool _hasStamina = playerStats.CheckHasStamina();
                
                if (!_hasStamina) {
                    b_Input = false;
                    SprintFlag = false;
                } else if (MoveAmount > 0.5f) {
                    SprintFlag = true;
                }
            }
            else {
                SprintFlag = false;
                if (RollInputTimer > 0 && RollInputTimer < 0.5f) {
                    RollFlag = true;
                }

                RollInputTimer = 0;
            }
        }

        private void HandleCombatInput() {
            // TODO: does this really belong here? also check for isInteracting
            // TODO: refactor this shit logic
            if (rb_Input) playerAttacker.HandleRBAction();

            if (lb_Input) playerAttacker.HandleLBAction();
            else {
                playerManager.IsBlocking = false;
                //TODO: this should just be an animation event
                playerEquipmentManager.CloseBlockingCollider();
            }
            
            if (rt_Input) {
                if (playerManager.CanDoCombo) {
                    ComboFlag = true;
                    playerAttacker.HandleWeaponCombo(playerInventory.RightWeapon);
                    ComboFlag = false;
                }
                else if (!playerManager.IsInteracting && !playerManager.CanDoCombo) {
                    playerAnimatorManager.Anim.SetBool(playerAnimatorManager.IsUsingRightHandHash, true);
                    playerAttacker.HandleHeavyAttack(playerInventory.RightWeapon);
                }
            }

            if (lt_Input) {
                //TODO: this is some shit placeholder logic
                if (TwoHandFlag) {
                    // If two handing handle ruightweapon art
                }
                else {
                    // handle weapon art if shield (parry)
                    playerAttacker.HandleLTAction();
                }
                // else handle light attack if melee weapon
                
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
            jump_Input = _callbackContext.action.WasPerformedThisFrame();
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

        private void HandleLockOnSwitch() {
            if (!LockOnFlag) return;
            if (!lockOn_right_Input && !lockOn_left_Input) return;
            
            cameraManager.HandleLockOn();
            cameraManager.SwitchLockOnTarget(lockOn_left_Input, lockOn_right_Input);
            lockOn_left_Input = false;
            lockOn_right_Input = false;
        }

        private void HandleTwoHandInput() {
            if (!twoHand_Input) return;

            twoHand_Input = false;
            TwoHandFlag = !TwoHandFlag;
            if (TwoHandFlag) {
                weaponSlotManager.LoadWeaponOnSlot(playerInventory.RightWeapon, false);
            }
            else {
                weaponSlotManager.LoadWeaponOnSlot(playerInventory.RightWeapon, false);
                weaponSlotManager.LoadWeaponOnSlot(playerInventory.LeftWeapon, true);
            }
        }

        private void HandleCriticalInput() {
            if (critical_Input) {
                critical_Input = false;
                playerAttacker.AttemptBackStabOrRiposte();
            }
        }
    }
}