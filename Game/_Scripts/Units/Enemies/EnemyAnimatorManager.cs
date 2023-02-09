using System;
using UnityEngine;

namespace Dzajna
{
    public class EnemyAnimatorManager : AnimatorManager
    {
        private Rigidbody rigidbody;
        
        private void Awake() {
            Anim = GetComponent<Animator>();
            rigidbody = GetComponent<Rigidbody>();
            IsInteractingHash = Animator.StringToHash("isInteracting");
        }

        private void OnAnimatorMove() {
            float delta = Time.deltaTime;
            Vector3 _deltaPos = Anim.deltaPosition;
            _deltaPos.y = 0;
            Vector3 _velocity = _deltaPos / delta;
            
            rigidbody.drag = 0;
            rigidbody.velocity = _velocity;
        }
    }
}