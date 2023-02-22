using UnityEngine;
using UnityEngine.Events;

namespace Dzajna {
public class AnimatorManager : MonoBehaviour {
    public Animator Anim;
    public int IsInteractingHash;
    public int IsDeadHash;
    public int CanRotateHash;
    public bool CanRotate;
    public int ParriedHash;
    public int ParryHash;

    public void PlayTargetAnimation(int _targetAnim, bool _isInteracting) {
        Anim.applyRootMotion = _isInteracting;
        Anim.SetBool(CanRotateHash, false);
        Anim.SetBool(IsInteractingHash, _isInteracting);
        Anim.CrossFade(_targetAnim, 0.2f);
    }
    
    public void PlayTargetAnimation(string _targetAnim, bool _isInteracting) {
        Anim.applyRootMotion = _isInteracting;
        Anim.SetBool(CanRotateHash, false);
        Anim.SetBool(IsInteractingHash, _isInteracting);
        Anim.CrossFade(_targetAnim, 0.2f);
    }

    public void EnableRotation() => Anim.SetBool(CanRotateHash, true);
    public void DisableRotation() => Anim.SetBool(CanRotateHash, false);
    public virtual void TakeCriticalDamageAnimEvent() { }
}
}