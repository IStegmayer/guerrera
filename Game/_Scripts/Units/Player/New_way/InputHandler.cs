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
        public float RollInputTimer;

        public bool b_Input;

        private PlayerInputActions inputActions;

        private Vector2 movementInput;
        private Vector2 cameraInput;


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
            MoveInput(_delta);
            HandleRollInput(_delta);
        }

        private void MoveInput(float _delta) {
            Horizontal = movementInput.x;
            Vertical = movementInput.y;
            MoveAmount = Mathf.Clamp01(Mathf.Abs(Horizontal) + Mathf.Abs(Vertical));
            CameraX = cameraInput.x;
            CameraY = cameraInput.y;
        }

        private void HandleRollInput(float _delta) {
            b_Input = inputActions.PlayerActions.Roll.IsPressed();

            if (b_Input) {
                RollInputTimer += _delta;
                SprintFlag = true;
            }
            else {
                if (RollInputTimer > 0 && RollInputTimer < 0.5f) {
                    SprintFlag = false;
                    RollFlag = true;
                }

                RollInputTimer = 0;
            }
        }
    }
}