using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dzajna {
public class PlayerCamera : PersistentSingleton<PlayerCamera> {
    [Header("References")] [SerializeField]
    private Transform cameraTransform;

    [SerializeField] private Transform cameraPivotTransform;
    [SerializeField] PlayerManager playerManager;

    [Header("Camera Settings")] [SerializeField]
    private float minPivot = -30;

    [SerializeField] private float maxPivot = 60;
    [SerializeField] private float leftAndRightRotationSpeed = 220;
    [SerializeField] private float upAndDownRotationSpeed = 220;

    [SerializeField]
    private float cameraSmoothSpeed = 1; // Bigger number means more smoothing. Change depending on different movesets

    [SerializeField] private float cameraCollisionRadius = 0.2f;
    [SerializeField] private LayerMask collideWithLayers;

    [Header("Camera Values")] [SerializeField]
    private float leftAndRightLookAngle;

    [SerializeField] private float upAndDownLookAngle;
    private float defaultCameraZPosition; // Values used for camera collisions
    private float targetCameraZPosition; // Values used for camera collisions
    private Vector3 cameraObjectPosition; // Used for camera collisions (moves the camera to this pos upon colliding)
    private Vector3 cameraVelocity;
    
    [Header("Lock on")] [SerializeField] private float maximumLockOnDistance = 30f;
    [SerializeField] private float lockedPivotPosition = 2.25f;
    [SerializeField] private float unlockedPivotPosition = 1.65f;
    public CharacterManager CurrentLockOnTarget;
    private CharacterManager nearestLockOnTarget;
    private CharacterManager leftLockOnTarget;
    private CharacterManager rightLockOnTarget;
    private List<CharacterManager> availableTargets = new List<CharacterManager>();

    protected override void Awake() {
        base.Awake();
    }

    private void Start() {
        defaultCameraZPosition = cameraTransform.localPosition.z;
    }

    private void FixedUpdate() {
        HandleAllCameraActions();
    }

    public void HandleAllCameraActions() {
        if (playerManager == null) return;
        HandleFollowTarget();
        HandleRotation();
        HandleCollisions();
    }

    private void HandleFollowTarget() {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, playerManager.transform.position,
            ref cameraVelocity, cameraSmoothSpeed * Time.fixedDeltaTime);
        transform.position = targetCameraPosition;
    }

    private void HandleRotation() {
        // IF LOCKED ON, FORCE ROTATION TOWARDS TARGET

        // Rotate based on horizontal/vertical movement of the joystick
        leftAndRightLookAngle += (InputHandler.Instance.CameraX * leftAndRightRotationSpeed) * Time.fixedDeltaTime;
        upAndDownLookAngle -= (InputHandler.Instance.CameraY * upAndDownRotationSpeed) * Time.fixedDeltaTime;
        upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minPivot, maxPivot);

        Vector3 cameraRotation = Vector3.zero;
        Quaternion targetRotation;

        // Rotate this gameObject left/right
        cameraRotation.y = leftAndRightLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        transform.rotation = Quaternion.Slerp
            (transform.rotation, targetRotation, cameraSmoothSpeed * Time.fixedDeltaTime);
        // Rotate the pivot up/right
        cameraRotation = Vector3.zero;
        cameraRotation.x = upAndDownLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        cameraPivotTransform.localRotation = Quaternion.Slerp
            (cameraPivotTransform.localRotation, targetRotation, cameraSmoothSpeed * Time.fixedDeltaTime);
    }

    private void HandleCollisions() {
        targetCameraZPosition = defaultCameraZPosition;
        RaycastHit hit;
        Vector3 rayDirection = cameraTransform.position - cameraPivotTransform.position;
        rayDirection.Normalize();

        // Check for collision with environment
        if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, rayDirection, out hit,
                Mathf.Abs(targetCameraZPosition), collideWithLayers)) {
            // Get distance from collision object
            float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
        }

        // Apply our offset to maintain radius distance (snap back)
        if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
            targetCameraZPosition = -cameraCollisionRadius;

        // Then we lerp to our new position
        cameraObjectPosition.z =
            Mathf.Lerp(cameraTransform.localPosition.z, targetCameraZPosition,
                cameraSmoothSpeed * Time.fixedDeltaTime);
        cameraObjectPosition.y = 0f;
        cameraTransform.localPosition = cameraObjectPosition;
    }
}
}