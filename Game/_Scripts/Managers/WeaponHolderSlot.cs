using UnityEngine;

namespace Dzajna
{
    public class WeaponHolderSlot : MonoBehaviour
    {
        public Transform parentOverride;
        public bool isLeftHandSlot;
        public bool isRightHandSlot;
        public bool isBackSlot;

        public GameObject CurrentWeaponModel;
        public WeaponItem CurrentWeaponItem;

        public void UnloadWeapon() {
            if (CurrentWeaponModel != null) CurrentWeaponModel.SetActive(false);
        }

        public void UnloadWeaponAndDestroy() {
            if (CurrentWeaponModel != null) Destroy(CurrentWeaponModel);
        }
        
        public void LoadWeaponModel(WeaponItem weaponItem) {
            UnloadWeaponAndDestroy();
            
            if (weaponItem == null) UnloadWeapon();
            
            GameObject model = Instantiate(weaponItem.ModelPrefab) as GameObject;
            if (model != null) {
                if (parentOverride != null) model.transform.parent = parentOverride;
                else model.transform.parent = transform;
                
                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;
                model.transform.localScale = Vector3.one;
            }

            CurrentWeaponModel = model;
        }
    }
}