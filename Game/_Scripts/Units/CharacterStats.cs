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
        
        protected int maxHealth;
        protected int currentHealth;
        protected int maxStamina;
        protected float currentStamina;
        protected float staminaRegenTimer;
        protected bool isDead;

    }
}