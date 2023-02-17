using UnityEngine;

namespace Dzajna
{
    public class CharacterManager : MonoBehaviour
    {
        [Header("Lock On Transform")]
        public Transform LockOnTransform;
        
        [Header("Combat Colliders")]
        public BoxCollider BackStabBoxCollider;

        public BackStabCollider BackStabCollider;

        // Damage will be inflicted during an animation event
        // Used in backstab or riposte animations
        public float PendingCriticalDamage;
    }
}