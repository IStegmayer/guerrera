using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dzajna {
public class OpenChest : Interactable {
    [SerializeField] private Transform playerStandingPosition;
    [SerializeField] private GameObject itemSpawner;
    [SerializeField] private WeaponItem itemInChest;
    
    private Animator animator;
    private OpenChest openChest;

    private void Awake() {
        animator = GetComponent<Animator>();
        openChest = GetComponent<OpenChest>();
    }

    public override void Interact(PlayerManager _playerManager) {
        base.Interact(_playerManager);
        Vector3 _rotationDirection = transform.position - _playerManager.transform.position;
        _rotationDirection.y = 0;
        _rotationDirection.Normalize();
        
        Quaternion _tr = Quaternion.LookRotation(_rotationDirection);
        Quaternion _targetRotation = Quaternion.Slerp(_playerManager.transform.rotation, _tr, 300 * Time.deltaTime);
        _playerManager.transform.rotation = _targetRotation;
        
        _playerManager.OpenChestInteraction(playerStandingPosition);
        animator.Play("OpenChest");
        StartCoroutine(SpawnItemInChest());

        WeaponPickUp _weaponPickUp = itemSpawner.GetComponent<WeaponPickUp>();
        _weaponPickUp.Weapon = itemInChest;
    }
    private IEnumerator SpawnItemInChest() {
        yield return new WaitForSeconds(1f);
        Instantiate(itemSpawner, transform);
        Destroy(openChest);
    }
}
}
