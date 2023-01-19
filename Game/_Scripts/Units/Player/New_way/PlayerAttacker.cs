using System;
using UnityEngine;

namespace Dzajna
{
    // TODO: All this hash management is very inefficient, weaponItem should have its animations already hashed 
    // also combo can just be an array of attacks and have a tree in anim. Overall a faulty script
    public class PlayerAttacker : MonoBehaviour
    {
        private AnimatorHandler animatorHandler;
        private InputHandler inputHandler;
        public string LastAttackString;
        
        private void Awake() {
            animatorHandler = GetComponent<AnimatorHandler>();
            inputHandler = GetComponent<InputHandler>();
        }

        public void HandleWeaponCombo(WeaponItem _weaponItem) {
            if (!inputHandler.ComboFlag) return;

            if (LastAttackString == _weaponItem.OH_Light_Attack_1) {
                animatorHandler.PlayTargetAnimation(Animator.StringToHash(_weaponItem.OH_Light_Attack_2), true);
            }
            else if (LastAttackString == _weaponItem.OH_Heavy_Attack_1) {
                animatorHandler.PlayTargetAnimation(Animator.StringToHash(_weaponItem.OH_Heavy_Attack_2), true);
            }

            animatorHandler.Anim.SetBool(Animator.StringToHash("canDoCombo"), false);
        }

        public void HandleLightAttack(WeaponItem _weaponItem) {
            int _attackHash = Animator.StringToHash(_weaponItem.OH_Light_Attack_1);
            animatorHandler.PlayTargetAnimation(_attackHash, true);
            LastAttackString = _weaponItem.OH_Light_Attack_1;
        }

        public void HandleHeavyAttack(WeaponItem _weaponItem) {
            // TODO: this is inefficient
            int _attackHash = Animator.StringToHash(_weaponItem.OH_Heavy_Attack_1);
            animatorHandler.PlayTargetAnimation(_attackHash, true);
            LastAttackString = _weaponItem.OH_Heavy_Attack_1;
        }
    }
}