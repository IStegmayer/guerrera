using System;
using UnityEngine;

namespace Dzajna
{
    public class PlayerAttacker : MonoBehaviour
    {
        private AnimatorHandler animatorHandler;

        private void Awake() {
            animatorHandler = GetComponent<AnimatorHandler>();
        }

        public void HandleLightAttack(WeaponItem _weaponItem) {
            // TODO: this is inefficient
            animatorHandler.PlayTargetAnimation(Animator.StringToHash(_weaponItem.OH_Light_Attack_1), true);
        }

        public void HandleHeavyAttack(WeaponItem _weaponItem) {
            // TODO: this is inefficient
            animatorHandler.PlayTargetAnimation(Animator.StringToHash(_weaponItem.OH_Heavy_Attack_1), true);
        }
    }
}