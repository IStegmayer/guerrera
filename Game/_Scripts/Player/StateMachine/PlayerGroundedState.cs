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
            Debug.Log("Exiting States");
        }

        public override void CheckSwitchStates() {
            if (Ctx.Input.Jump && !Ctx.RequireNewJumpPress) {
                SwitchState(Factory.Jump());
            } else if (!Ctx.CharacterController.isGrounded) {
                SwitchState(Factory.Fall());
            }
        }

        public override void InitializeSubState() {
            if (!Ctx.IsMovementPressed && !Ctx.Input.Run) {
                SetSubState(Factory.Idle());
            }else if (Ctx.IsMovementPressed && !Ctx.Input.Run) {
                SetSubState(Factory.Walk());
            }
            else {
                SetSubState((Factory.Run()));
            }
        }

        public void HandleGravity() {
            Ctx.CurrentMovementY = Ctx.Gravity;
            Ctx.AppliedMovementY = Ctx.Gravity;
        }
    }
}