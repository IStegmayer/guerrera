using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dzajna {
public class CharacterSFXManager : MonoBehaviour {
    [SerializeField] private Sound[] characterSounds;
    [SerializeField] private Sound[] damageSounds;
    [SerializeField] private Sound deathSound;
    [SerializeField] private Sound staggerSound;
    private Sound[] weaponWhooshes;
    private AudioSource audioSource;
    private CharacterManager character;

    private bool isDoingACriticalHit;
    private int lastSwooshSFXIndex = -1;
    private int lastDamageSFXIndex = -1;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        character = GetComponent<CharacterManager>();
    }

    private void Start() {
        weaponWhooshes = character.CharacterInventoryManager.RightWeapon.WeaponWhooshes;
    }

    // Animation events
    public void PlayWeaponSound() => PlaySound(PickRandomSound(weaponWhooshes, ref lastSwooshSFXIndex));
    public void PlayDamageSound() => PlaySound(PickRandomSound(damageSounds, ref lastDamageSFXIndex));
    public void PlayStaggerSound() => PlaySound(staggerSound);
    public void PlayDeathSound() => PlaySound(deathSound);
    public void PlayFootstepSound() {
        if (!character.IsInteracting) PlaySound(FindSoundByName("footstep"));
    }
    public void PlayRollSound() => PlaySound(FindSoundByName("roll"));
    public void PlayCriticalSound() {
        if (!isDoingACriticalHit) {
            PlaySound(FindSoundByName("criticalHit"));
            isDoingACriticalHit = true;
        }
        else {
            PlaySound(FindSoundByName("criticalRelease"));
            isDoingACriticalHit = false;
        }
    }
    
    // Code methods
    public void PlayParrySound() => PlaySound(FindSoundByName("parry"));
    public void PlayBlockSound() => PlaySound(FindSoundByName("block"));
    
    
    public void PlaySound(Sound soundToPlay) {
        audioSource.clip = soundToPlay.Clip;
        audioSource.loop = soundToPlay.Loop;
        audioSource.volume = soundToPlay.Volume;

        if (!audioSource.loop)
            audioSource.PlayOneShot(audioSource.clip, audioSource.volume);
        else
            audioSource.Play();
    }

    private Sound PickRandomSound(Sound[] potentialSounds, ref int lastSoundIndex) {
        int tempIndex = lastSoundIndex;
        lastSoundIndex = Enumerable.Range(0, potentialSounds.Length).FirstOrDefault(_ => _ != tempIndex);
        
        return potentialSounds[lastSoundIndex];
    }
    private Sound FindSoundByName(string soundName) {
        Sound s = Array.Find(characterSounds, sound => sound.Name == soundName);
        if (s == null) {
            Debug.LogWarning("Character sound: " + soundName + " not found!");
            return null;
        }

        return s;
    }

    //TODO: Use this to update sounds after weapon change
    public void UpdatePotentialWeaponWhooshes(Sound[] newSounds) => weaponWhooshes = newSounds;
}
}