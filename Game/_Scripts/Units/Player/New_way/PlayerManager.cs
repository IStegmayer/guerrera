using System;
using System.Collections;
using UnityEngine;

namespace Dzajna {
public class PlayerManager : CharacterManager {
    private Animator anim;
    private PlayerAnimatorManager playerAnimatorManager;
    private PlayerCamera playerCamera;
    private InputHandler inputHandler;
    private PlayerLocomotionManager playerLocomotionManager;
    private PlayerStats playerStats;

    private InteractableUI interactableUI;

    private void Awake() {
        playerCamera = PlayerCamera.Instance;
        inputHandler = InputHandler.Instance;
        interactableUI = FindObjectOfType<InteractableUI>();
    }

    private void Start() {
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
        IsInAir = anim.GetBool(playerAnimatorManager.IsInAirHash);
        IsUsingLeftHand = anim.GetBool(playerAnimatorManager.IsUsingLeftHandHash);
        IsUsingRightHand = anim.GetBool(playerAnimatorManager.IsUsingRightHandHash);
        IsInvulnerable = anim.GetBool(playerAnimatorManager.IsInvulnerableHash);
        IsFiringSpell = anim.GetBool(playerAnimatorManager.IsFiringSpellHash);
        playerAnimatorManager.CanRotate = anim.GetBool(playerAnimatorManager.CanRotateHash);
        playerAnimatorManager.Anim.SetBool(playerAnimatorManager.IsDeadHash, playerStats.IsDead);
        playerAnimatorManager.Anim.SetBool(playerAnimatorManager.IsBlockingHash, IsBlocking);

        inputHandler.TickInput(delta);

        CheckForInteractableObject();
        playerLocomotionManager.HandleMovement(delta);
        playerLocomotionManager.HandleRotation(delta);
        playerLocomotionManager.HandleRolling(delta);
        // TODO: WTF CALLING with its own property? 
        playerLocomotionManager.HandleFalling(delta, playerLocomotionManager.moveDirection);
        playerLocomotionManager.HandleJumping();

        playerStats.RegenerateStamina();
    }

    private void FixedUpdate() {
        if (playerCamera != null) playerCamera.HandleAllCameraActions();
    }

    private void LateUpdate() {
        if (IsInAir) playerLocomotionManager.InAirTimer += Time.deltaTime;
    }

    //TODO: This needs to change anyway 
    private void CheckForInteractableObject() {
        RaycastHit hit;
        Interactable interactable;

        //TODO: not perfect, probable an overlapsphere or just ontriggerEnter/Exit would be better
        if (!Physics.SphereCast(transform.position, 1f, transform.forward, out hit, 1)) return;
        if (!hit.transform.TryGetComponent(out interactable)) {
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
        playerLocomotionManager.Rigidbody.velocity = Vector3.zero;
        transform.position = playerStandingPoint.transform.position;
        playerAnimatorManager.PlayTargetAnimation(playerAnimatorManager.OpenChestHash, true);
    }

    public void PassThroughFogWallInteraction(Transform fogWallEntrance) {
        playerLocomotionManager.Rigidbody.velocity = Vector3.zero;
        Vector3 rotationDirection = fogWallEntrance.transform.forward;
        Quaternion turnRotation = Quaternion.LookRotation(rotationDirection);
        transform.rotation = turnRotation;

        playerAnimatorManager.PlayTargetAnimation(playerAnimatorManager.PassThroughFogWallHash, true);
    }
}
}