using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;


namespace Game._Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        // constants
        private float _rotationFactorPerFrame = 10.0f;
        private float _runMultiplier = 3.0f;
        private float _gravity = -9.8f;
        private float _groundedGravity = -.05f;

        // references and class fields
        private CharacterController _characterController;
        private Animator _animator;

        private FrameInput _input;

        // move fields
        private Vector3 _currentMovement;
        private Vector3 _currentRunMovement;
        private Vector3 _appliedMovement;

        private bool _isMovementPressed;

        // jump fields
        private bool _isJumping = false;
        private float _initialJumpVelocity;
        private float _maxJumpHeight = 3f;
        private float _maxJumpTime = 0.75f;
        private float _fallTerminalVelocity = -20.0f;
        private int _jumpCount = 0;
        private Dictionary<int, float> _initialJumpVelocities = new Dictionary<int, float>();
        private Dictionary<int, float> _jumpGravities = new Dictionary<int, float>();
        private Coroutine _currentJumpResetCoroutine;

        private void Start() {
            // set reference variables
            _characterController = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
            SetupJumpVariables();
        }

        private void SetupJumpVariables() {
            float timeToApex = _maxJumpTime / 2;
            _gravity = (-2 * _maxJumpHeight) / Mathf.Pow(timeToApex, 2);
            _initialJumpVelocity = (2 * _maxJumpHeight) / timeToApex;
            float secondJumpGravity = (-2 * (_maxJumpHeight + 2)) / MathF.Pow((timeToApex * 1.25f), 2);
            float secondJumpInitialVelocity = (2 * (_maxJumpHeight + 2)) / (timeToApex * 1.25f);
            float thirdJumpGravity = (-2 * (_maxJumpHeight + 4)) / MathF.Pow((timeToApex * 1.5f), 2);
            float thirdJumpInitialVelocity = (2 * (_maxJumpHeight + 4)) / (timeToApex * 1.5f);

            _initialJumpVelocities.Add(1, _initialJumpVelocity);
            _initialJumpVelocities.Add(2, secondJumpInitialVelocity);
            _initialJumpVelocities.Add(3, thirdJumpInitialVelocity);

            _jumpGravities.Add(0, _gravity);
            _jumpGravities.Add(1, _gravity);
            _jumpGravities.Add(2, secondJumpGravity);
            _jumpGravities.Add(3, thirdJumpGravity);
        }

        private void HandleRotation() {
            if (!_isMovementPressed) return;

            // get current rotation and movement
            Quaternion currentRotation = transform.rotation;
            Vector3 positionToLookAt;
            positionToLookAt.x = _currentMovement.x;
            positionToLookAt.y = 0.0f;
            positionToLookAt.z = _currentMovement.z;

            // slerp and execute rotation
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation =
                Quaternion.Slerp(currentRotation, targetRotation, _rotationFactorPerFrame * Time.deltaTime);
            ;
        }

        private void HandleGravity() {
            bool isFalling = _currentMovement.y <= 0.0f || !_input.Jump;
            float fallMultiplier = 2.5f;
            if (_characterController.isGrounded) {
                _currentMovement.y = _groundedGravity;
                _appliedMovement.y = _groundedGravity;
            }
            else if (isFalling) {
                float previousYVelocity = _currentMovement.y;
                _currentMovement.y = 
                    _currentMovement.y + (_jumpGravities[_jumpCount] * fallMultiplier * Time.deltaTime);
                _appliedMovement.y = MathF.Max((previousYVelocity + _currentMovement.y) * .5f, _fallTerminalVelocity);
            }
            else {
                float previousYVelocity = _currentMovement.y;
                _currentMovement.y = _currentMovement.y + (_jumpGravities[_jumpCount] * Time.deltaTime);
                _appliedMovement.y = (previousYVelocity + _currentMovement.y) * .5f;
            }
        }

        private void HandleJump() {
            if (!_isJumping && _characterController.isGrounded && _input.Jump) {
                if (_jumpCount < 3 && _currentJumpResetCoroutine != null) {
                    StopCoroutine(_currentJumpResetCoroutine);
                    _currentJumpResetCoroutine = null;
                }
                else if (_jumpCount == 3) _jumpCount = 0;

                _jumpCount++;
                _animator.Play("Jump");
                _isJumping = true;
                _currentMovement.y = _initialJumpVelocities[_jumpCount];
                _appliedMovement.y = _initialJumpVelocities[_jumpCount];
            }
            else if (_isJumping && _characterController.isGrounded) {
                _isJumping = false;
                _animator.Play("Idle");
            }
        }
        
        private void Update() {
            HandleRotation();

            if (_input.Run) {
                _appliedMovement.x = _currentRunMovement.x;
                _appliedMovement.z = _currentRunMovement.z;
            }
            else {
                _appliedMovement.x = _currentMovement.x;
                _appliedMovement.z = _currentMovement.z;
            }

            _characterController.Move(_appliedMovement * Time.deltaTime);
            
            HandleGravity();
            HandleJump();
            Debug.Log("_jumpCount" + _jumpCount);
        }

        #region GatherInput

        // handler for movement vars
        public void OnMove(InputAction.CallbackContext context) {
            _input.Movement = context.ReadValue<Vector2>();
            _currentMovement.x = _input.Movement.x;
            _currentMovement.z = _input.Movement.y;
            _currentRunMovement.x = _input.Movement.x * _runMultiplier;
            _currentRunMovement.z = _input.Movement.y * _runMultiplier;
            _isMovementPressed = _input.Movement.x != 0 || _input.Movement.y != 0;
        }

        public void OnRun(InputAction.CallbackContext context) =>
            _input.Run = context.action.IsPressed();

        public void OnJump(InputAction.CallbackContext context) {
            _input.Jump = context.action.WasPressedThisFrame();
        }

        #endregion
    }
}