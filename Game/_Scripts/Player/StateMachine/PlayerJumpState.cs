using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;


namespace Game._Scripts.Player.StateMachine
{
    public class PlayerJumpState : PlayerBaseState
    {
        public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
            : base(currentContext, playerStateFactory) {
            IsRootState = true;
        }

        private IEnumerator JumpResetRoutine() {
            yield return new WaitForSeconds(.5f);
            Ctx.JumpCount = 0;
        }
        
        public override void EnterState() {
            if (Ctx.Input.Jump) HandleJump();
        }

        public override void UpdateState() {
            CheckSwitchStates();
            HandleGravity();
        }

        public override void ExitState() {
            if (Ctx.Input.Jump) Ctx.RequireNewJumpPress = true;
            Ctx.CurrentJumpResetCoroutine = Ctx.StartCoroutine(JumpResetRoutine());
            if (Ctx.JumpCount == 3) Ctx.JumpCount = 0;
        }

        public override void CheckSwitchStates() {
            if (Ctx.CharacterController.isGrounded) {
                SwitchState(Factory.Grounded());
            }
        }

        public override void InitializeSubState() {
            throw new System.NotImplementedException();
        }

        private void HandleJump() {
            if (Ctx.JumpCount < 3 && Ctx.CurrentJumpResetCoroutine != null) {
                Ctx.StopCoroutine(Ctx.CurrentJumpResetCoroutine);
                Ctx.CurrentJumpResetCoroutine = null;
            }
            
            Ctx.JumpCount++;
            Ctx.Animator.Play("Jump");
            Ctx.IsJumping = true;
            Ctx.CurrentMovementY = Ctx.InitialJumpVelocities[Ctx.JumpCount];
            Ctx.AppliedMovementY = Ctx.InitialJumpVelocities[Ctx.JumpCount];
        }

        private void HandleGravity() {
            bool isFalling = Ctx.CurrentMovementY <= 0.0f || !Ctx.Input.Jump;
            float fallMultiplier = 2.5f;
            if (isFalling) {
                float previousYVelocity = Ctx.CurrentMovementY;
                Ctx.CurrentMovementY = 
                    Ctx.CurrentMovementY + (Ctx.JumpGravities[Ctx.JumpCount] * fallMultiplier * Time.deltaTime);
                Ctx.AppliedMovementY = MathF.Max((previousYVelocity + Ctx.CurrentMovementY) * .5f, Ctx.FallTerminalVelocity);
            }
            else {
                float previousYVelocity = Ctx.CurrentMovementY;
                Ctx.CurrentMovementY = Ctx.CurrentMovementY + (Ctx.JumpGravities[Ctx.JumpCount] * Time.deltaTime);
                Ctx.AppliedMovementY = (previousYVelocity + Ctx.CurrentMovementY) * .5f;
            }
        }
    }
}
