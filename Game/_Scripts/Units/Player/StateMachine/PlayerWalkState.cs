using UnityEngine;


namespace Game._Scripts.Player.StateMachine
{
    public class PlayerWalkState : PlayerBaseState
    {
        public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
            : base(currentContext, playerStateFactory) { }

        public override void EnterState() {
            Ctx.Animator.SetBool("isWalking", true);
        }

        public override void UpdateState() {
            Ctx.AppliedMovementX = Ctx.Input.Movement.x * Ctx.WalkMultiplier;
            Ctx.AppliedMovementZ = Ctx.Input.Movement.y * Ctx.WalkMultiplier;
            CheckSwitchStates();
        }

        public override void ExitState() {
            Ctx.Animator.SetBool("isWalking", false);
        }

        public override void CheckSwitchStates() {
            if (Ctx.Input.Attack) {
                SwitchState(Factory.Attack());
            }
            else if (!Ctx.IsMovementPressed) {
                SwitchState(Factory.Idle());
            } else if (Ctx.Input.Run) {
                SwitchState(Factory.Run());
            }
        }

        public override void InitializeSubState() {
            throw new System.NotImplementedException();
        }
    }
}