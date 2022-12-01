using UnityEngine;


namespace Game._Scripts.Player.StateMachine
{
    public class PlayerIdleState : PlayerBaseState
    {
        public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)  
            : base(currentContext, playerStateFactory) { }

        public override void EnterState() {
            Debug.Log("Entered Idle state (animate this)");
        }

        public override void UpdateState() {
            CheckSwitchStates();
            Ctx.AppliedMovementX = 0.0f;
            Ctx.AppliedMovementZ = 0.0f;
        }

        public override void ExitState() {
            Debug.Log("Exiting Idle state");
        }

        public override void CheckSwitchStates() {
            if (Ctx.IsMovementPressed && Ctx.Input.Run) {
                SwitchState(Factory.Run());
            } else if (Ctx.IsMovementPressed) {
                SwitchState(Factory.Walk());
            }
        }

        public override void InitializeSubState() {
            throw new System.NotImplementedException();
        }
    }
}