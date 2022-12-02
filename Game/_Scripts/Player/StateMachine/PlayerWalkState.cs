using UnityEngine;


namespace Game._Scripts.Player.StateMachine
{
    public class PlayerWalkState : PlayerBaseState
    {
        public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
            : base(currentContext, playerStateFactory) { }

        public override void EnterState() {
            Debug.Log("Entered Walk state (animate this)");
        }

        public override void UpdateState() {
            Ctx.AppliedMovementX = Ctx.Input.Movement.x;
            Ctx.AppliedMovementZ = Ctx.Input.Movement.y;
            CheckSwitchStates();
        }

        public override void ExitState() {
            Debug.Log("Exiting Walk state");
        }

        public override void CheckSwitchStates() {
            if (!Ctx.IsMovementPressed) {
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