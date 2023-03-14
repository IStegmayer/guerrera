using UnityEngine;

namespace Dzajna
{
    public class CharacterStats : MonoBehaviour
    {
        //TODO: ofc this is all simplified
        [SerializeField] protected int healthLevel = 10;
        [SerializeField] protected int healthPerLevel = 5;
        [SerializeField] protected int staminaLevel = 10;
        [SerializeField] protected int staminaPerLevel = 5;
        [SerializeField] protected float staminaRegenAmount = 2f;
        [SerializeField] protected int focusLevel = 10;
        [SerializeField] protected int focusPerLevel = 5;
        
        protected float maxHealth;
        protected float currentHealth;
        protected float maxStamina;
        protected float currentStamina;
        protected float maxFP;
        protected float currentFP;
        protected float staminaRegenTimer;
        public bool IsDead;

        public virtual void TakeDamage(float _dmg, string _dmgAnimation = "Damage1"){}
    }
}