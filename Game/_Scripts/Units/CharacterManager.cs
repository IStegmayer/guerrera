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

        // Damage will be inflicted during an animation event
        // Used in backstab or riposte animations
        public float PendingCriticalDamage;
    }
}