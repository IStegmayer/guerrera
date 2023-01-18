using UnityEngine;

namespace Dzajna
{
    public class WeaponHolderSlot : MonoBehaviour
    {
        public Transform parentOverride;
        public bool isLeftHandSlot;
        public bool isRightHandSlot;

        public GameObject CurrentWeaponModel;

        public void UnloadWeapon() {
            if (CurrentWeaponModel != null) CurrentWeaponModel.SetActive(false);
        }

        public void UnloadWeaponAndDestroy() {
            if (CurrentWeaponModel != null) Destroy(CurrentWeaponModel);
        }
        
        public void LoadWeaponModel(WeaponItem _weaponItem) {
            UnloadWeaponAndDestroy();
            
            if (_weaponItem == null) UnloadWeapon();
            
            GameObject _model = Instantiate(_weaponItem.modelPrefab) as GameObject;
            if (_model != null) {
                if (parentOverride != null) _model.transform.parent = parentOverride;
                else _model.transform.parent = transform;
                
                _model.transform.localPosition = Vector3.zero;
                _model.transform.localRotation = Quaternion.identity;
                _model.transform.localScale = Vector3.one;
            }

            CurrentWeaponModel = _model;
        }
    }
}