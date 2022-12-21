using System.Collections;
using UnityEngine;

namespace Game._Scripts.Player.StateMachine
{
    public class PlayerAttackState : PlayerBaseState
    {
        public PlayerAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(
            currentContext, playerStateFactory) {
        }


        private IEnumerator SlashResetRoutine() {
            yield return new WaitForSeconds(1f);
            // modify slash count
            Ctx.IsSlashing = false;
        }

        public override void EnterState() {
            Ctx.IsSlashing = true;
            Ctx.Animator.Play("Slash");
            Ctx.StartCoroutine(SlashResetRoutine());
        }

        public override void UpdateState() {
            Ctx.AppliedMovementX = 0.0f;
            Ctx.AppliedMovementZ = 0.0f;
            CheckSwitchStates();
        }

        public override void ExitState() {
            Ctx.Animator.Play("Idle");
        }

        public override void CheckSwitchStates() {
            if (!Ctx.IsSlashing) SwitchState(Factory.Idle());
        }

        public override void InitializeSubState() {
        }
    }
}