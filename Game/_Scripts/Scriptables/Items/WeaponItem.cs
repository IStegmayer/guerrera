using UnityEngine;

namespace Dzajna
{
    [CreateAssetMenu(menuName = "Items/Weapon Item")]
    public class WeaponItem : Item
    {
        public GameObject ModelPrefab;
        public bool IsUnarmed;

        [Header("Damage")] 
        public int BaseDamage;
        public int CriticalDamageMultiplier;

        [Header("Idle Animations")] 
        public string OH_RightArmIdle;
        public string OH_LeftArmIdle;
        public string TH_Idle;
        
        [Header("One Handed Attack Animations")]
        public string OH_Light_Attack_1;
        public string OH_Light_Attack_2;
        public string OH_Heavy_Attack_1;
        public string OH_Heavy_Attack_2;

        [Header("Two Handed Attack Animations")]
        public string TH_Light_Attack_1;
        public string TH_Light_Attack_2;
        public string TH_Heavy_Attack_1;
        public string TH_Heavy_Attack_2;

        [Header("Weapon Art")] 
        public string WeaponArt;

        [Header("Stamina Costs")] 
        public int BaseStamina;
        public float LightAttackMultiplier;
        public float HeavyAttackMultiplier;

        [Header("Weapon Type")] 
        public bool isMagicCaster;
        public bool isFaithCaster;
        public bool isPyroCaster;
        public bool isMeleeWeapon;
        public bool isShieldWeapon;


    }
}