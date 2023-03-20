using System;
using UnityEngine;
using UnityEngine.Events;

namespace Dzajna {
public class AnimatorManager : MonoBehaviour {
    public CharacterManager characterManager;
    public Animator Anim;
    public int VerticalHash;
    public int HorizontalHash;
    public int IsInteractingHash;
    public int IsDeadHash;
    public int IsBlockingHash;
    public int CanRotateHash;
    public int IsFiringSpellHash;
    public int IsRotatingWithRMHash;
    public bool CanRotate;
    public int ParriedHash;
    public int ParryHash;
    public int CanDoComboHash;
    public int IsInvulnerableHash;
    public int BlockHash;
    public int BlockDamage1Hash;
    public int Damage1Hash;
    public int TurnAroundHash;
    public int TurnRightHash;
    public int TurnLeftHash;


    public void PlayTargetAnimation(int _targetAnim, bool _isInteracting, bool _canRotate = false) {
        Anim.applyRootMotion = _isInteracting;
        Anim.SetBool(CanRotateHash, _canRotate);
        Anim.SetBool(IsInteractingHash, _isInteracting);
        Anim.CrossFade(_targetAnim, 0.2f);
    }

    public void PlayTargetAnimation(string _targetAnim, bool _isInteracting, bool _canRotate = false) {
        Anim.applyRootMotion = _isInteracting;
        Anim.SetBool(CanRotateHash, _canRotate);
        Anim.SetBool(IsInteractingHash, _isInteracting);
        Anim.CrossFade(_targetAnim, 0.2f);
    }

    public void PlayTargetAnimationWithRootRotation(int _targetAnim, bool _isInteracting) {
        Anim.applyRootMotion = _isInteracting;
        Anim.SetBool(IsRotatingWithRMHash, true);
        Anim.SetBool(IsInteractingHash, _isInteracting);
        Anim.CrossFade(_targetAnim, 0.2f);
    }

    public void EnableRotation() => Anim.SetBool(CanRotateHash, true);
    public void DisableRotation() => Anim.SetBool(CanRotateHash, false);
    public void EnableCombo() => Anim.SetBool(CanDoComboHash, true);
    public void DisableCombo() => Anim.SetBool(CanDoComboHash, false);
    public void EnableIsInvulnerable() => Anim.SetBool(IsInvulnerableHash, true);
    public void DisableIsInvulnerable() => Anim.SetBool(IsInvulnerableHash, false);
    public void EnableIsParrying() => characterManager.IsParrying = true;
    public void DisableIsParrying() => characterManager.IsParrying = false;
    public void EnableCanBeRiposted() => characterManager.CanBeRiposted = true;
    public void DisableCanBeRiposted() => characterManager.CanBeRiposted = false;
    public virtual void TakeCriticalDamageAnimEvent() { }
}
}