using UnityEngine;


namespace Game._Scripts.Player.StateMachine
{
    public class PlayerRunState : PlayerBaseState
    {
        public PlayerRunState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
            : base(currentContext, playerStateFactory) {
        }

        public override void EnterState() {
            Debug.Log("Entering Run state (animate this)");
        }

        public override void UpdateState() {
            Ctx.AppliedMovementX = Ctx.Input.Movement.x * Ctx.RunMultiplier;
            Ctx.AppliedMovementZ = Ctx.Input.Movement.y * Ctx.RunMultiplier;
            CheckSwitchStates();
        }

        public override void ExitState() {
            Debug.Log("Exiting Run state");
        }

        public override void CheckSwitchStates() {
            if (!Ctx.IsMovementPressed) {
                SwitchState(Factory.Idle());
            }
            else if (!Ctx.Input.Run) {
                SwitchState(Factory.Run());
            }
        }

        public override void InitializeSubState() {
            throw new System.NotImplementedException();
        }
    }
}