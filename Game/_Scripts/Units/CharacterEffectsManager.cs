using System;
using UnityEngine;

namespace Dzajna {
public class CharacterEffectsManager : MonoBehaviour {
    [Header("Damage FX")] 
    [SerializeField] private GameObject bloodSplatterFX;
    private Vector3 lastHitPosition;

    public WeaponFX RightWeaponFX { get; set; }
    public WeaponFX LeftWeaponFX { get; set; }

    public virtual void PlayRightWeaponFX() {
        if (RightWeaponFX != null) RightWeaponFX.PlayWeaponFX();
    }

    public virtual void PlayLeftWeaponFX() {
        if (LeftWeaponFX != null) LeftWeaponFX.PlayWeaponFX();
    }

    public virtual void StopRightWeaponFX() {
        if (RightWeaponFX != null) RightWeaponFX.StopWeaponFX();
    }

    public virtual void StopLeftWeaponFX() {
        if (LeftWeaponFX != null) LeftWeaponFX.StopWeaponFX();
    }

    public virtual void PlayBloodSplatterFX(Vector3 bloodSplatterPos) {
        GameObject blood = Instantiate(bloodSplatterFX, bloodSplatterPos, Quaternion.identity);
    }
    public virtual void PlayCriticalBloodSplatterFX() {
        GameObject blood = Instantiate(bloodSplatterFX, lastHitPosition, Quaternion.identity);
    }

    public virtual void SetLastHitPosition(Vector3 pos) => lastHitPosition = pos;
}
}