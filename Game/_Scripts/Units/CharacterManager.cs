using System;
using UnityEngine;

namespace Dzajna
{
    public class CharacterManager : MonoBehaviour
    {
        [Header("Lock On Transform")]
        public Transform LockOnTransform;
        
        [Header("Combat Colliders")]
        public CriticalDamageCollider BackStabCollider;
        public CriticalDamageCollider RiposteCollider;

        [Header("Combat Flags")] 
        public bool CanBeRiposted;
        public bool CanBeParried;
        public bool CanDoCombo;
        public bool IsParrying;
        public bool IsBlocking;

        [Header("MovementFlags")] 
        public bool IsRotatingWithRM;
        
        [Header("Spell Flags")]
        public bool IsFiringSpell;

        // Damage will be inflicted during an animation event
        // Used in backstab or riposte animations
        public float PendingCriticalDamage;
    }
}