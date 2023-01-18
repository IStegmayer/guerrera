using UnityEngine;

namespace Dzajna
{
    [CreateAssetMenu(menuName = "Items/Weapon Item")]
    public class WeaponItem : Item
    {
        public GameObject modelPrefab;
        public bool isUnarmed;

        [Header("Idle Animations")] 
        public string OH_RightArmIdle;
        public string OH_LeftArmIdle;
        
        [Header("One Handed Attack Animations")]
        public string OH_Light_Attack_1;
        public string OH_Heavy_Attack_1;
    }
}