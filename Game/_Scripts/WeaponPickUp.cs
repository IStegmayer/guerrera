using System.Collections;
using UnityEngine;

namespace Dzajna
{
    public class WeaponPickUp : Interactable
    {
        public WeaponItem Weapon;

        public override void Interact(PlayerManager _playerManager) {
            base.Interact(_playerManager);
            PickUpItem(_playerManager);
        }

        private void PickUpItem(PlayerManager _playerManager) {
            PlayerInventoryManager _playerInventoryManager;
            PlayerLocomotionManager _playerLocomotionManager;
            PlayerAnimatorManager _playerAnimatorManager;
            InteractableUI _interactableUI;
            _playerInventoryManager = _playerManager.GetComponent<PlayerInventoryManager>();
            _playerLocomotionManager = _playerManager.GetComponent<PlayerLocomotionManager>();
            _playerAnimatorManager = _playerManager.GetComponent<PlayerAnimatorManager>();
            _interactableUI = FindObjectOfType<InteractableUI>();
            
            
            _playerLocomotionManager.Rigidbody.velocity = Vector3.zero; // Stop movement
            _playerAnimatorManager.PlayTargetAnimation(Animator.StringToHash("PickUpItem"), true);
            _playerInventoryManager.WeaponsInventory.Add(Weapon);
            _interactableUI.ItemPickupText.text = Weapon.ItemName;
            _interactableUI.ItemPickupImage.texture = Weapon.ItemIcon.texture;
            _interactableUI.ItemPickupUIGameObject.SetActive(true);
            GetComponent<Collider>().enabled = false;
            StartCoroutine(DismissItemPickupRoutine(_interactableUI));
        }
        
        private IEnumerator DismissItemPickupRoutine(InteractableUI _interactableUI) {
            yield return new WaitForSeconds(2f);
            _interactableUI.ItemPickupUIGameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}