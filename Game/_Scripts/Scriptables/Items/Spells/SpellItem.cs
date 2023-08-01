using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dzajna {
public class SpellItem : Item {
    [SerializeField] protected GameObject spellWarmUpFX;
    [SerializeField] protected GameObject spellCastFX;
    [SerializeField] protected string spellAnimation;

    [Header("FP cost")] // 
    public int FPCost;

    [Header("Spell Type")] // 
    public bool isFaithSpell;
    public bool isMagicSpell;
    public bool isPyroSpell;

    [Header("Spell Description")] //
    [TextArea] [SerializeField] protected string spellDescription;

    [Header("Spell Sounds")] //
    [SerializeField] protected Sound attemptSFX;
    [SerializeField] protected Sound successSFX;

    public virtual void AttemptToCastSpell(CharacterManager character) {
        Debug.Log("Attempting to cast spell");
        character.CharacterSfxManager.PlaySound(attemptSFX);
    }

    public virtual void SuccessfullyCastedSpell(CharacterManager character) {
        Debug.Log("Successfully casted a spell");
        character.CharacterSfxManager.PlaySound(successSFX);
        PlayerStatsManager playerStatsManager = character.CharacterStatsManager as PlayerStatsManager;
        if (playerStatsManager != null) playerStatsManager.TakeFPDamage(FPCost);
    }
}
}