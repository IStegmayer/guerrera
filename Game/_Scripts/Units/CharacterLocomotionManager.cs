using System;
using UnityEngine;

namespace Dzajna {
public class CharacterLocomotionManager : MonoBehaviour {
    public Vector3 moveDirection;
    protected CharacterController characterController;
    protected CharacterManager character;
    protected CharacterAnimatorManager CharacterAnimatorManager;

    [Header("Gravity Settings")] [SerializeField]
    private LayerMask groundLayer;

    [SerializeField] protected Vector3 fallVelocity;
    [SerializeField] protected float fallStartYVelocity = -7f;
    [SerializeField] protected float groundedYVelocity = -20f;
    [SerializeField] protected float gravityForce = -9.8f;
    protected float groundCheckSphereRadius;
    protected Vector3 groundCheckStartingPosition;
    protected bool fallingVelocitySet = false;

    public float InAirTimer;
    
    protected virtual void Start() {
        character = GetComponent<CharacterManager>();
        CharacterAnimatorManager = character.GetComponent<CharacterAnimatorManager>();
        characterController = GetComponent<CharacterController>();
        groundCheckSphereRadius = characterController.radius;
    }

    protected virtual void Update() {
        groundCheckStartingPosition = transform.position;
        groundCheckStartingPosition.y += groundCheckSphereRadius - 0.05f;
        character.IsGrounded = Physics.CheckSphere(groundCheckStartingPosition, groundCheckSphereRadius, groundLayer);
        CharacterAnimatorManager.Anim.SetBool(CharacterAnimatorManager.IsGroundedHash, character.IsGrounded);
        HandleGroundCheck();
    }

    public virtual void HandleGroundCheck() {
        if (character.IsGrounded && fallVelocity.y < 0) {
            InAirTimer = 0;
            fallingVelocitySet = false;
            fallVelocity.y = groundedYVelocity;
        }
        else {
            if (!fallingVelocitySet) {
                fallingVelocitySet = true;
                fallVelocity.y = fallStartYVelocity;
            }

            InAirTimer = InAirTimer + Time.deltaTime;
            fallVelocity.y += gravityForce * Time.deltaTime;
        }

        CharacterAnimatorManager.Anim.SetFloat(CharacterAnimatorManager.InAirTimer, InAirTimer);
        character.CharacterController.Move(fallVelocity * Time.deltaTime);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawSphere(groundCheckStartingPosition, groundCheckSphereRadius);
    }
}
}