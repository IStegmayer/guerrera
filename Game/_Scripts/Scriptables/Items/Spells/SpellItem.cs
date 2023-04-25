using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dzajna {
public class SpellItem : Item {
    [SerializeField] protected GameObject spellWarmUpFX;
    [SerializeField] protected GameObject spellCastFX;
    [SerializeField] protected string spellAnimation;

    [Header("FP cost")] public int FPCost;

    [Header("Spell Type")] public bool isFaithSpell;
    public bool isMagicSpell;
    public bool isPyroSpell;

    [Header("Spell Description")] [TextArea] [SerializeField]
    protected string spellDescription;

    public virtual void AttemptToCastSpell(AnimatorManager animatorManager, PlayerStats playerStats,
        PlayerCamera playerCamera,
        PlayerWeaponSlotManager playerWeaponSlotManager) {
        Debug.Log("Attempting to cast spell");
    }

    public virtual void SuccessfullyCastedSpell(AnimatorManager animatorManager, PlayerStats playerStats,
        PlayerCamera playerCamera, PlayerWeaponSlotManager playerWeaponSlotManager) {
        Debug.Log("Successfully casted a spell");
        playerStats.TakeFPDamage(FPCost);
    }
}
}