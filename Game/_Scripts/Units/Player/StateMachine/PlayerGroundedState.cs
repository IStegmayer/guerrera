using System;
using UnityEngine;


namespace Game._Scripts.Player.StateMachine
{
    public class PlayerGroundedState : PlayerBaseState, IRootState
    {
        public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
            : base(currentContext, playerStateFactory) {
            IsRootState = true;
        }

        public override void EnterState() {
            InitializeSubState();
            HandleGravity();
        }

        public override void UpdateState() {
            CheckSwitchStates();
        }

        public override void ExitState() {
            Debug.Log("Exiting CurrentGameState grounded");
        }

        public override void CheckSwitchStates() {
            if (Ctx.Input.Jump && !Ctx.RequireNewJumpPress) {
                SwitchState(Factory.Jump());
            }
            else if (!Ctx.CharacterController.isGrounded) {
                SwitchState(Factory.Fall());
            }
        }

        public override void InitializeSubState() {
            if (Ctx.Input.Attack) {
                SetSubState(Factory.Attack());
            }
            else if (!Ctx.IsMovementPressed && !Ctx.Input.Run) {
                Ctx.Animator.SetBool("isRunning", false);
                Ctx.Animator.SetBool("isWalking", false);
                SetSubState(Factory.Idle());
            }
            else if (Ctx.IsMovementPressed && !Ctx.Input.Run) {
                Ctx.Animator.SetBool("isWalking", true);
                Ctx.Animator.SetBool("isRunning", false);
                SetSubState(Factory.Walk());
            }
            else {
                Ctx.Animator.SetBool("isRunning", true);
                Ctx.Animator.SetBool("isWalking", false);
                SetSubState((Factory.Run()));
            }
        }

        public void HandleGravity() {
            Ctx.CurrentMovementY = Ctx.Gravity;
            Ctx.AppliedMovementY = Ctx.Gravity;
        }
    }
}