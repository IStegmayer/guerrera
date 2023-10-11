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
    private Quaternion initialRotation;
    private OpenDoor openDoor;
    private AudioSource audioSource;

    private void Start() {
        initialRotation = rightDoor.rotation;
        openDoor = GetComponent<OpenDoor>();
        audioSource = GetComponent<AudioSource>();
    }

    public override void Interact(PlayerManager playerManager) {
        base.Interact(playerManager);

        playerManager.CharacterController.enabled = false;
        playerManager.transform.position = playerStandingPosition.position;
        playerManager.transform.rotation = playerStandingPosition.rotation;
        playerManager.CharacterController.enabled = true;
        
        CharacterAnimatorManager anim = playerManager.GetComponent<CharacterAnimatorManager>();
        anim.PlayTargetAnimation("OpenDoubleDoor", true);
        audioSource.clip = openClip;
        audioSource.PlayDelayed(0.5f);
        // animate door
        StartCoroutine(OpenDoors());
    } 
    
    private IEnumerator OpenDoors()
    {
        float timeElapsed = 0.0f;
        float volume = AudioSystem.Instance.masterVolume;
        
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
        Destroy(openDoor, 0.5f);
    }
}
}