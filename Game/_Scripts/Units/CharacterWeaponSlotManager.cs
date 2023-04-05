using UnityEngine;

namespace Dzajna {
public class CharacterWeaponSlotManager : MonoBehaviour {
    [Header("Weapons")]
    [SerializeField] protected WeaponItem rightHandWeapon;
    [SerializeField] protected WeaponItem leftHandWeapon;
    [SerializeField] protected WeaponItem unarmedWeapon;
    
    protected WeaponHolderSlot rightHandSlot;
    protected WeaponHolderSlot leftHandSlot;
    protected WeaponHolderSlot backSlot;
    
    public DamageCollider RightDamageCollider { get; protected set; }
    public DamageCollider LeftDamageCollider { get; protected set; }
    
}
}