using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

namespace Dzajna {
public class InputHandler : PersistentSingleton<InputHandler> {
    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }
    public float MoveAmount { get; private set; }
    public float CameraX { get; private set; }
    public float CameraY { get; private set; }

    public bool b_Input { get; private set; }
    public bool a_Input { get; private set; }
    public bool rb_Input { get; private set; }
    public bool rt_Input { get; private set; }
    public bool lt_Input { get; private set; }
    public bool lb_Input { get; private set; }
    public bool critical_Input { get; private set; }
    public bool jump_Input { get; private set; }
    public bool twoHand_Input { get; private set; }
    public bool inventory_Input { get; private set; }
    public bool lockOn_Input { get; private set; }
    public bool lockOn_left_Input { get; private set; }
    public bool lockOn_right_Input { get; private set; }

    public bool d_pad_up_Input { get; private set; }
    public bool d_pad_down_Input { get; private set; }
    public bool d_pad_left_Input { get; private set; }
    public bool d_pad_right_Input { get; private set; }

    private float rollInputTimer;


    public bool SprintFlag { get; private set; }
    public bool RollFlag { get; private set; }
    public bool ComboFlag { get; private set; }
    public bool InventoryFlag { get; private set; }
    public bool LockOnFlag { get; private set; }
    public bool TwoHandFlag { get; private set; }

    [SerializeField] private PlayerManager playerManager;
    private PlayerAnimatorManager playerAnimatorManager;
    private PlayerInputActions inputActions;
    private PlayerCombatManager playerCombatManager;
    private PlayerInventoryManager playerInventoryManager;
    private PlayerStats playerStats;
    private PlayerEquipmentManager playerEquipmentManager;
    private PlayerCamera playerCamera;
    private UIManager uiManager;
    private PlayerWeaponSlotManager playerWeaponSlotManager;

    private Vector2 movementInput;
    private Vector2 cameraInput;

    protected override void Awake() {
        base.Awake();
        playerCombatManager = playerManager.GetComponent<PlayerCombatManager>();
        playerInventoryManager = playerManager.GetComponent<PlayerInventoryManager>();
        playerAnimatorManager = playerManager.GetComponent<PlayerAnimatorManager>();
        playerStats = playerManager.GetComponent<PlayerStats>();
        playerWeaponSlotManager = playerManager.GetComponentInChildren<PlayerWeaponSlotManager>();
        playerEquipmentManager = playerManager.GetComponentInChildren<PlayerEquipmentManager>();
        // TODO: inefficient asf
        playerCamera = PlayerCamera.Instance;
        uiManager = FindObjectOfType<UIManager>();
    }

    public void OnEnable() {
        // read input and populate our fields
        if (inputActions == null) {
            inputActions = new PlayerInputActions();

            inputActions.PlayerMovement.Movement.performed += input => movementInput = input.ReadValue<Vector2>();
            inputActions.PlayerMovement.Camera.performed += input => cameraInput = input.ReadValue<Vector2>();

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

    private void LateUpdate() {
        //reset flags
        RollFlag = false;
        SprintFlag = false;
        rb_Input = false;
        rt_Input = false;
        lt_Input = false;
        a_Input = false;
        jump_Input = false;
        lockOn_Input = false;
        d_pad_right_Input = false;
        d_pad_left_Input = false;
        d_pad_up_Input = false;
        d_pad_down_Input = false;
    }

    public void TickInput(float delta) {
        HandleMoveInput();
        HandleRollInput(delta);
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

    private void HandleRollInput(float delta) {
        if (b_Input) {
            rollInputTimer += delta;
            bool hasStamina = playerStats.CheckHasStamina();

            if (!hasStamina) {
                b_Input = false;
                SprintFlag = false;
            }
            else if (MoveAmount > 0.5f) {
                SprintFlag = true;
            }
        }
        else {
            SprintFlag = false;
            if (rollInputTimer > 0 && rollInputTimer < 0.5f) {
                RollFlag = true;
            }

            rollInputTimer = 0;
        }
    }

    private void HandleCombatInput() {
        // TODO: does this really belong here? also check for isInteracting
        // TODO: refactor this shit logic
        if (rb_Input) playerCombatManager.HandleRBAction();

        if (lb_Input) playerCombatManager.HandleLBAction();
        else {
            playerManager.IsBlocking = false;
            //TODO: this should just be an animation event
            playerEquipmentManager.CloseBlockingCollider();
        }

        if (rt_Input) {
            if (playerManager.CanDoCombo) {
                ComboFlag = true;
                playerCombatManager.HandleWeaponCombo(playerInventoryManager.RightWeapon);
                ComboFlag = false;
            }
            else if (!playerManager.IsInteracting && !playerManager.CanDoCombo) {
                playerAnimatorManager.Anim.SetBool(playerAnimatorManager.IsUsingRightHandHash, true);
                playerCombatManager.HandleHeavyAttack(playerInventoryManager.RightWeapon);
            }
        }

        if (lt_Input) {
            //TODO: this is some shit placeholder logic
            if (TwoHandFlag) {
                // If two handing handle ruightweapon art
            }
            else {
                // handle weapon art if shield (parry)
                playerCombatManager.HandleLTAction();
            }
            // else handle light attack if melee weapon
        }
    }

    private void HandleQuickSlotsInput() {
        if (d_pad_right_Input) {
            playerInventoryManager.ChangeRightWeapon();
        }
        else if (d_pad_left_Input) {
            playerInventoryManager.ChangeLeftWeapon();
        }
    }

    private void HandleInteractInput(CallbackContext callbackContext) {
        a_Input = callbackContext.action.WasPerformedThisFrame();
    }

    private void HandleJumpInput(CallbackContext callbackContext) {
        jump_Input = callbackContext.action.WasPerformedThisFrame();
    }

    private void HandleInventoryInput(CallbackContext callbackContext) {
        inventory_Input = callbackContext.action.WasPerformedThisFrame();
        if (!inventory_Input) return;

        InventoryFlag = !InventoryFlag;
        uiManager.ToggleSelectWindow(InventoryFlag);
        if (InventoryFlag) uiManager.UpdateUI();
        else uiManager.CloseAllInventoryWindows();
        uiManager.ToggleHUDWindow(!InventoryFlag);
    }

    private void HandleLockOnInput() {
        if (!lockOn_Input) return;

        // if (!LockOnFlag) {
        //     lockOn_Input = false;
        //     playerCamera.HandleLockOn();
        //     LockOnFlag = playerCamera.SetCurrentLockOnTarget();
        // }
        // else {
        //     lockOn_Input = false;
        //     LockOnFlag = false;
        //     playerCamera.ClearLockOnTargets();
        // }
    }

    private void HandleLockOnSwitch() {
        if (!LockOnFlag) return;
        if (!lockOn_right_Input && !lockOn_left_Input) return;

        // playerCamera.HandleLockOn();
        // playerCamera.SwitchLockOnTarget(lockOn_left_Input, lockOn_right_Input);
        // lockOn_left_Input = false;
        // lockOn_right_Input = false;
    }

    private void HandleTwoHandInput() {
        if (!twoHand_Input) return;

        twoHand_Input = false;
        TwoHandFlag = !TwoHandFlag;
        if (TwoHandFlag) {
            playerWeaponSlotManager.LoadWeaponOnSlot(playerInventoryManager.RightWeapon, false);
        }
        else {
            playerWeaponSlotManager.LoadWeaponOnSlot(playerInventoryManager.RightWeapon, false);
            playerWeaponSlotManager.LoadWeaponOnSlot(playerInventoryManager.LeftWeapon, true);
        }
    }

    private void HandleCriticalInput() {
        if (critical_Input) {
            critical_Input = false;
            playerCombatManager.AttemptBackStabOrRiposte();
        }
    }

    public void SetComboFlag(bool val) => ComboFlag = val;
}
}