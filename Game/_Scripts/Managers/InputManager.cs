using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dzajna
{
    public class InputManager : MonoBehaviour
    {
        public float Horizontal;
        public float Vertical;
        public float MoveAmount;
        public float CameraX;
        public float CameraY;

        private PlayerInputActions _inputActions;
        private CameraManager _cameraManager;

        private Vector2 _movementInput;
        private Vector2 _cameraInput;

        private void Awake() {
            _cameraManager = CameraManager.Instance;
        }

        private void FixedUpdate() {
            float delta = Time.fixedDeltaTime;
            if (_cameraManager != null) {
                _cameraManager.FollowTarget(delta);
                _cameraManager.HandleCameraRotation(delta, CameraX, CameraY);
            }
        }

        public void OnEnable() {
            // read input and populate our fields
            if (_inputActions == null) {
                _inputActions = new PlayerInputActions();
                _inputActions.PlayerControls.Move.performed += input => _movementInput = input.ReadValue<Vector2>();
                _inputActions.PlayerControls.Camera.performed += input => _cameraInput = input.ReadValue<Vector2>();
            }
            
            // enable our created inputActions 
            _inputActions.Enable();
        }

        public void OnDisable() {
            // making sure to cleanup
            _inputActions.Disable();
        }

        //TODO: this should be called in player update
        public void TickInput(float delta) {
            MoveInput(delta);
        }
        public void MoveInput(float delta) {
            Horizontal = _movementInput.x;
            Vertical = _movementInput.y;
            MoveAmount = Mathf.Clamp01(Mathf.Abs(Horizontal) + Mathf.Abs(Vertical));
            CameraX = _cameraInput.x;
            CameraY = _cameraInput.y;
        }
    }
}