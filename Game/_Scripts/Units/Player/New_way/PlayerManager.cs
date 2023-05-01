using System;
using System.Collections;
using UnityEngine;

namespace Dzajna {
public class PlayerManager : CharacterManager {
    private Animator anim;
    private PlayerAnimatorManager playerAnimatorManager;
    private InputHandler inputHandler;
    private PlayerLocomotionManager playerLocomotionManager;
    private PlayerStats playerStats;

    private InteractableUI interactableUI;

    [Header("Interact settings")] 
    [SerializeField] private float interactRange;
    [SerializeField] private LayerMask interactableLayer;

    protected override void Awake() {
        base.Awake();
        interactableUI = FindObjectOfType<InteractableUI>();
    }

    protected override void Start() {
        base.Start();
        inputHandler = InputHandler.Instance;
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        anim = GetComponent<Animator>();
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerStats = GetComponent<PlayerStats>();
        BackStabCollider = GetComponentInChildren<CriticalDamageCollider>();
    }

    private void Update() {
        float delta = Time.deltaTime;
        IsInteracting = anim.GetBool(playerAnimatorManager.IsInteractingHash);
        CanDoCombo = anim.GetBool(playerAnimatorManager.CanDoComboHash);
        IsUsingLeftHand = anim.GetBool(playerAnimatorManager.IsUsingLeftHandHash);
        IsUsingRightHand = anim.GetBool(playerAnimatorManager.IsUsingRightHandHash);
        IsInvulnerable = anim.GetBool(playerAnimatorManager.IsInvulnerableHash);
        IsFiringSpell = anim.GetBool(playerAnimatorManager.IsFiringSpellHash);
        playerAnimatorManager.CanRotate = anim.GetBool(playerAnimatorManager.CanRotateHash);
        playerAnimatorManager.Anim.SetBool(playerAnimatorManager.IsDeadHash, playerStats.IsDead);
        playerAnimatorManager.Anim.SetBool(playerAnimatorManager.IsBlockingHash, IsBlocking);

        inputHandler.TickInput(delta);

        playerLocomotionManager.HandleGroundMovement(delta);
        playerLocomotionManager.HandleRotation(delta);
        playerLocomotionManager.HandleRolling(delta);
        playerLocomotionManager.HandleJumping();
        
        CheckForInteractableObject();

        playerStats.RegenerateStamina();
    }

    private void FixedUpdate() {
    }

    //TODO: This needs to change anyway 
    private void CheckForInteractableObject() {
        Interactable interactable;
        RaycastHit hit;

        Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, interactRange, interactableLayer);
        // TODO: inefficient
        if (!hit.transform || !hit.transform.TryGetComponent(out interactable)) {
            if (interactableUI.InteractableUIGameObject != null)
                interactableUI.InteractableUIGameObject.SetActive(false);
            return;
        }

        string interactableText = interactable.InteractableText;
        interactableUI.InteractableText.text = interactableText;
        interactableUI.InteractableUIGameObject.SetActive(true);

        if (inputHandler.a_Input) {
            interactable.Interact(this);
        }
    }

    public void OpenChestInteraction(Transform playerStandingPoint) {
        transform.position = playerStandingPoint.transform.position;
        playerAnimatorManager.PlayTargetAnimation(playerAnimatorManager.OpenChestHash, true);
    }

    public void PassThroughFogWallInteraction(Transform fogWallEntrance) {
        Vector3 rotationDirection = fogWallEntrance.transform.forward;
        Quaternion turnRotation = Quaternion.LookRotation(rotationDirection);
        transform.rotation = turnRotation;

        playerAnimatorManager.PlayTargetAnimation(playerAnimatorManager.PassThroughFogWallHash, true);
    }
}
}