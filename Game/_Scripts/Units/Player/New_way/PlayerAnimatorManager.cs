using System;
using UnityEngine;

namespace Dzajna {
public class PlayerAnimatorManager : AnimatorManager {
    // TODO: public?
    private PlayerManager playerManager;
    private PlayerLocomotionManager playerLocomotionManager;
    private PlayerStats playerStats;


    protected override void Awake() {
        base.Awake();
        playerManager = GetComponent<PlayerManager>();
        Anim = GetComponent<Animator>();
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerStats = GetComponent<PlayerStats>();
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
        playerLocomotionManager.Rigidbody.drag = 0;
        Vector3 _deltaPosition = Anim.deltaPosition;
        _deltaPosition.y = 0;
        Vector3 _velocity = _deltaPosition / _delta;
        playerLocomotionManager.Rigidbody.velocity = _velocity;
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