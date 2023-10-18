using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Dzajna {
public class OpenDoor : Interactable {
    [SerializeField] private Transform playerStandingPosition;
    [SerializeField] private Transform rightDoor;
    [SerializeField] private Transform leftDoor;
    [SerializeField] private float animDuration;
    [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private AudioClip openClip;
    [SerializeField] private AudioClip endClip;
    [SerializeField] private string musicToPlay;
    private Quaternion initialRotation;
    private OpenDoor openDoor;
    private AudioSource audioSource;
    private AudioSystem audioSystem;

    private bool isOpeningDoor;

    private WeaponItem rightWeapon;
    private WeaponItem leftWeapon;

    private void Start() {
        initialRotation = rightDoor.rotation;
        openDoor = GetComponent<OpenDoor>();
        audioSource = GetComponent<AudioSource>();
        audioSystem = AudioSystem.Instance;
    }

    public override void Interact(PlayerManager playerManager) {
        if (isOpeningDoor) return;
        isOpeningDoor = true;

        base.Interact(playerManager);

        rightWeapon = playerManager.CharacterInventoryManager.RightWeapon;
        leftWeapon = playerManager.CharacterInventoryManager.LeftWeapon;

        playerManager.CharacterController.enabled = false;
        playerManager.transform.position = playerStandingPosition.position;
        playerManager.transform.rotation = playerStandingPosition.rotation;
        playerManager.CharacterController.enabled = true;

        CharacterAnimatorManager anim = playerManager.GetComponent<CharacterAnimatorManager>();
        anim.PlayTargetAnimation("OpenDoubleDoor", true);
        anim.Anim.SetBool(anim.IsInvulnerableHash, true);
        
        audioSource.clip = openClip;
        audioSource.PlayDelayed(0.5f);

        UnequipWeapons(playerManager);
        // animate door
        StartCoroutine(OpenDoors(playerManager));
        StartCoroutine(ReEquipWeapons(playerManager));
    }

    private IEnumerator OpenDoors(PlayerManager playerManager) {
        float timeElapsed = 0.0f;
        float volume = audioSystem.masterVolume;
        
        while (timeElapsed < animDuration) {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / animDuration);
            float curveValue = animationCurve.Evaluate(t);

            audioSource.volume = curveValue * volume;
            Quaternion targetRotation = initialRotation * Quaternion.Euler(0, -curveValue * 80.0f, 0);
            leftDoor.rotation = targetRotation;
            targetRotation = initialRotation * Quaternion.Euler(0, curveValue * 80.0f, 0);
            rightDoor.rotation = targetRotation;
            yield return null;
        }

        audioSource.Stop();
        audioSource.clip = endClip;
        audioSource.Play();
        isOpeningDoor = false;
        
        yield return null;
        audioSystem.PlayMusic(musicToPlay);
        Destroy(openDoor, 0.5f);
    }

    private void UnequipWeapons(PlayerManager playerManager) {
        playerManager.CharacterWeaponSlotManager.LoadWeaponOnSlot(null, true);
        playerManager.CharacterWeaponSlotManager.LoadWeaponOnSlot(null, false);
        playerManager.CharacterWeaponSlotManager.LoadWeaponsOnBackSlots(leftWeapon, rightWeapon);
    }

    private IEnumerator ReEquipWeapons(PlayerManager playerManager) {
        yield return new WaitForSeconds(animDuration * 0.9f);
        playerManager.CharacterWeaponSlotManager.UnloadBackSlots();
        playerManager.CharacterWeaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
        playerManager.CharacterWeaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
    }
}
}