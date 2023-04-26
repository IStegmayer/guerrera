using System;
using UnityEngine;
using UnityEngine.Events;

namespace Dzajna {
public class AnimatorManager : MonoBehaviour {
    public CharacterManager characterManager;
    public Animator Anim;
    public bool CanRotate;
    
    // properties hashes
    public int IsGroundedHash;
    public int IsUsingRightHandHash;
    public int IsUsingLeftHandHash;
    public int VerticalHash;
    public int HorizontalHash;
    public int IsInteractingHash;
    public int IsDeadHash;
    public int IsBlockingHash;
    public int CanRotateHash;
    public int IsFiringSpellHash;
    public int IsRotatingWithRMHash;
    public int IsPhaseShiftingHash;
    public int CanDoComboHash;
    public int IsInvulnerableHash;
    public int InAirTimer;

    // states hashes
    public int RollHash;
    public int BackstepHash;
    public int LandHash;
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
    public int PassThroughFogWallHash;
    public int BlockHash;
    public int BlockDamage1Hash;
    public int Damage1Hash;
    public int TurnAroundHash;
    public int TurnRightHash;
    public int TurnLeftHash;
    public int PhaseTransitionHash;
    public int ParriedHash;
    public int ParryHash;
    
    protected virtual void Awake() {
        VerticalHash = Animator.StringToHash("Vertical");
        HorizontalHash = Animator.StringToHash("Horizontal");
        IsInteractingHash = Animator.StringToHash("isInteracting");
        IsDeadHash = Animator.StringToHash("isDead");
        CanRotateHash = Animator.StringToHash("canRotate");
        ParriedHash = Animator.StringToHash("Parried");
        ParryHash = Animator.StringToHash("Parry");
        CanDoComboHash = Animator.StringToHash("canDoCombo");
        IsInvulnerableHash = Animator.StringToHash("isInvulnerable");
        IsRotatingWithRMHash = Animator.StringToHash("isRotatingWithRM");
        TurnAroundHash = Animator.StringToHash("TurnAround");
        TurnLeftHash = Animator.StringToHash("TurnLeft");
        TurnRightHash = Animator.StringToHash("TurnRight");
        PhaseTransitionHash = Animator.StringToHash("PhaseTransition");
        IsPhaseShiftingHash = Animator.StringToHash("isPhaseShifting");
        IsUsingRightHandHash = Animator.StringToHash("isUsingRightHand");
        IsUsingLeftHandHash = Animator.StringToHash("isUsingLeftHand");
        IsBlockingHash = Animator.StringToHash("isBlocking");
        IsFiringSpellHash = Animator.StringToHash("isFiringSpell");
        RollHash = Animator.StringToHash("Roll");
        BackstepHash = Animator.StringToHash("Backstep");
        LandHash = Animator.StringToHash("Land");
        FallHash = Animator.StringToHash("Fall");
        EmptyHash = Animator.StringToHash("Empty");
        Damage1Hash = Animator.StringToHash("Damage1");
        BlockDamage1Hash = Animator.StringToHash("BlockDamage1");
        DeathHash = Animator.StringToHash("Death");
        JumpHash = Animator.StringToHash("Jump");
        FailedCastHash = Animator.StringToHash("FailedCast");
        IsGroundedHash = Animator.StringToHash("isGrounded");
        InAirTimer = Animator.StringToHash("inAirTimer");
        BackStabHash = Animator.StringToHash("BackStab");
        BackStabbedHash = Animator.StringToHash("BackStabbed");
        BlockHash = Animator.StringToHash("Block");
        RiposteHash = Animator.StringToHash("Riposte");
        RipostedHash = Animator.StringToHash("Riposted");
        OpenChestHash = Animator.StringToHash("OpenChest");
        PassThroughFogWallHash = Animator.StringToHash("PassThroughFogWall");
    }

    public void PlayTargetAnimation(int targetAnim, bool isInteracting, bool canRotate = false) {
        Anim.applyRootMotion = isInteracting;
        Anim.SetBool(CanRotateHash, canRotate);
        Anim.SetBool(IsInteractingHash, isInteracting);
        Anim.CrossFade(targetAnim, 0.2f);
    }

    public void PlayTargetAnimation(string targetAnim, bool isInteracting, bool canRotate = false) {
        Anim.applyRootMotion = isInteracting;
        Anim.SetBool(CanRotateHash, canRotate);
        Anim.SetBool(IsInteractingHash, isInteracting);
        Anim.CrossFade(targetAnim, 0.2f);
    }

    public void PlayTargetAnimationWithRootRotation(int targetAnim, bool isInteracting) {
        Anim.applyRootMotion = isInteracting;
        Anim.SetBool(IsRotatingWithRMHash, true);
        Anim.SetBool(IsInteractingHash, isInteracting);
        Anim.CrossFade(targetAnim, 0.2f);
    }
    
    //reposition character model when using root motion
    protected virtual void OnAnimatorMove() {
        if (characterManager.IsInteracting == false) return;

        Vector3 deltaPosition = Anim.deltaPosition;
        characterManager.CharacterController.Move(deltaPosition);
        characterManager.transform.rotation *= Anim.deltaRotation;
        
        // if (aiCharacter.isRotatingWithRootMotion)
        // aiCharacter.transform.rotation *= Anim.deltaRotation;
    }

    //TODO: ALL EVENTS SHOULD BE HERE 
    
    public void EnableRotation() => Anim.SetBool(CanRotateHash, true);
    public void DisableRotation() => Anim.SetBool(CanRotateHash, false);
    public void EnableCombo() => Anim.SetBool(CanDoComboHash, true);
    public void DisableCombo() => Anim.SetBool(CanDoComboHash, false);
    public void EnableIsInvulnerable() => Anim.SetBool(IsInvulnerableHash, true);
    public void DisableIsInvulnerable() => Anim.SetBool(IsInvulnerableHash, false);
    //TODO: does this actually work?
    public void EnableIsParrying() => characterManager.IsParrying = true;
    public void DisableIsParrying() => characterManager.IsParrying = false;
    public void EnableCanBeRiposted() => characterManager.CanBeRiposted = true;
    public void DisableCanBeRiposted() => characterManager.CanBeRiposted = false;
    public virtual void TakeCriticalDamageAnimEvent() { }
}
}