using UnityEngine;

namespace Dzajna
{
    public class AnimatorManager : MonoBehaviour
    {
        public Animator Anim;
        public int IsInteractingHash;
        public void PlayTargetAnimation(int _targetAnim, bool _isInteracting) {
            Anim.applyRootMotion = _isInteracting;
            Anim.SetBool(IsInteractingHash, _isInteracting);
            Anim.CrossFade(_targetAnim, 0.2f);
        }
        
        public void PlayTargetAnimation(string _targetAnim, bool _isInteracting) {
            Anim.applyRootMotion = _isInteracting;
            Anim.SetBool(IsInteractingHash, _isInteracting);
            Anim.CrossFade(_targetAnim, 0.2f);
        }

    }
}