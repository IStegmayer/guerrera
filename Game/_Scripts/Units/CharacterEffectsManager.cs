using UnityEngine;

namespace Dzajna {
public class CharacterEffectsManager : MonoBehaviour {
    public WeaponFX RightWeaponFX;
    public WeaponFX LeftWeaponFX;

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
}
}