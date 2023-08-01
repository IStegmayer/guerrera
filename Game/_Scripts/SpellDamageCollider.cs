using System;
using UnityEngine;

namespace Dzajna {
public class SpellDamageCollider : DamageCollider {
    public GameObject ImpactParticles;
    public GameObject MuzzleParticles;
    public Sound explosionSound;

    private bool hasCollided;
    private CharacterManager spellTarget;
    private AudioSource audioSource;
    private ParticleSystem particleSystem;
    private Vector3 impactNormal; //Used to rotate impact particles

    private void Start() {

        if (MuzzleParticles) {
            Instantiate(MuzzleParticles, transform.position, transform.rotation);
        }
    }

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnTriggerEnter(Collider other) {
        if (hasCollided) return;

        spellTarget = other.transform.GetComponent<CharacterManager>();
        if (spellTarget != null) {
            if (spellTarget.IsInvulnerable) return;
            
            spellTarget.CharacterStatsManager.TakeDamage(PhysicalDamage);
        }
        
        hasCollided = true;
        audioSource.PlayOneShot(explosionSound.Clip, explosionSound.Volume);
        ImpactParticles = Instantiate(ImpactParticles, transform.position,
            Quaternion.FromToRotation(Vector3.up, impactNormal));
        
        particleSystem.Stop();
        Destroy(gameObject,1f);
    }
}
}