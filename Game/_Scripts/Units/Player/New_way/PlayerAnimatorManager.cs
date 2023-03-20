using System;
using UnityEngine;

namespace Dzajna
{
    public class PlayerAnimatorManager : AnimatorManager
    {
        // TODO: public?
        private PlayerManager playerManager;
        private InputHandler inputHandler;
        private PlayerLocomotion playerLocomotion;
        private PlayerStats playerStats;
        
        // properties hashes
        public int IsInAirHash;
        public int IsUsingRightHandHash;
        public int IsUsingLeftHandHash;
        
        // states hashes
        public int RollHash;
        public int BackstepHash;
        public int LandHash;
        public int LocomotionHash;
        public int FallHash;
        public int EmptyHash;
        public int DeathHash;
        public int JumpHash;
        public int FailedCastHash;
        public int BackStabHash;
        public int RiposteHash;
        public int RipostedHash;
        public int BackStabbedHash;
        public int OpenChestHash;
        
        public void Initialize() {
            playerManager = GetComponent<PlayerManager>();
            Anim = GetComponent<Animator>();
            inputHandler = GetComponent<InputHandler>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
            playerStats = GetComponent<PlayerStats>(); 
            VerticalHash = Animator.StringToHash("Vertical");
            HorizontalHash = Animator.StringToHash("Horizontal");
            IsInteractingHash = Animator.StringToHash("isInteracting");
            CanDoComboHash = Animator.StringToHash("canDoCombo");
            IsUsingRightHandHash = Animator.StringToHash("isUsingRightHand");
            IsUsingLeftHandHash = Animator.StringToHash("isUsingLeftHand");
            IsInvulnerableHash = Animator.StringToHash("isInvulnerable");
            IsBlockingHash = Animator.StringToHash("isBlocking");
            IsFiringSpellHash = Animator.StringToHash("isFiringSpell");
            CanRotateHash = Animator.StringToHash("canRotate");
            IsDeadHash = Animator.StringToHash("isDead");
            RollHash = Animator.StringToHash("Roll");
            BackstepHash = Animator.StringToHash("Backstep");
            LandHash = Animator.StringToHash("Land");
            LocomotionHash = Animator.StringToHash("Locomotion");
            FallHash = Animator.StringToHash("Fall");
            EmptyHash = Animator.StringToHash("Empty");
            Damage1Hash = Animator.StringToHash("Damage1");
            BlockDamage1Hash = Animator.StringToHash("BlockDamage1");
            DeathHash = Animator.StringToHash("Death");
            JumpHash = Animator.StringToHash("Jump");
            FailedCastHash = Animator.StringToHash("FailedCast");
            IsInAirHash = Animator.StringToHash("isInAir");
            BackStabHash = Animator.StringToHash("BackStab");
            BackStabbedHash = Animator.StringToHash("BackStabbed");
            ParriedHash = Animator.StringToHash("Parried");
            ParryHash = Animator.StringToHash("Parry");
            BlockHash = Animator.StringToHash("Block");
            RiposteHash = Animator.StringToHash("Riposte");
            RipostedHash = Animator.StringToHash("Riposted");
            OpenChestHash = Animator.StringToHash("OpenChest");
            CanRotate = true;
        }

        public void UpdateAnimatorValues(float _verticalMovement, float _horizontalMovement, bool _isSprinting) {
            //clamp values
            // #region Vertical
            //
            // float _vertical = 0;
            //
            // if (_verticalMovement > 0 && _verticalMovement < 0.55f) _vertical = 0.5f;
            // else if (_verticalMovement > 0.55f) _vertical = 1f;
            // else if (_verticalMovement < 0 && _verticalMovement > -0.55f) _vertical = -0.5f;
            // else if (_verticalMovement < -0.55f) _vertical = -1f;
            // else _vertical = 0f;
            //
            // #endregion
            // #region Horizontal
            //
            // float _horizontal = 0;
            //
            // if (_horizontalMovement > 0 && _horizontalMovement < 0.55f) _horizontal = 0.5f;
            // else if (_horizontalMovement > 0.55f) _horizontal = 1f;
            // else if (_horizontalMovement < 0 && _horizontalMovement > -0.55f) _horizontal = -0.5f;
            // else if (_horizontalMovement < -0.55f) _horizontal = -1f;
            // else _horizontal = 0f;
            // #endregion

            float _horizontal = _horizontalMovement;
            float _vertical = _verticalMovement;

            if (_isSprinting) {
                _vertical = 2;
                _horizontal = _horizontalMovement;
            }
            
            Anim.SetFloat(VerticalHash, _vertical, 0.1f, Time.deltaTime);
            Anim.SetFloat(HorizontalHash, _horizontal, 0.1f, Time.deltaTime);
            Anim.applyRootMotion = playerManager.IsInteracting;
        }

        //reposition character model when using root motion
        private void OnAnimatorMove() {
            if (playerManager.IsInteracting == false) return;
        
            float _delta = Time.deltaTime;
            playerLocomotion.Rigidbody.drag = 0;
            Vector3 _deltaPosition = Anim.deltaPosition;
            _deltaPosition.y = 0;
            Vector3 _velocity = _deltaPosition / _delta;
            playerLocomotion.Rigidbody.velocity = _velocity;
        }
        
        public void AwardSoulsOnDeath() {
            //TODO:  Placeholder for enemynimator event.
        }

        public override void TakeCriticalDamageAnimEvent() {
            playerStats.TakeDamageWithoutAnimation(playerManager.PendingCriticalDamage);
            playerManager.PendingCriticalDamage = 0;
        }
    }
}