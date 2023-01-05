using UnityEngine;

// Class made to check weapon colliding and weapon-specific damage
// also works as a DamageDealer class
public class Weapon : MonoBehaviour
{
    private BaseUnit _unit;
    //TODO: This should be a class managed by the Unit itself using scriptableObjects
    // but this'll do for now
    [SerializeField] private int _weaponDmg = 2;
    private bool _canDealDamage;

    public void ToggleEnableDamage(bool state) => _canDealDamage = state;
    
    private void Awake() {
        _unit = transform.root.gameObject.GetComponentInChildren<BaseUnit>();
    }

    private void OnCollisionEnter(Collision other) {
        if (!_canDealDamage) return;
        
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            BaseUnit enemy = other.gameObject.GetComponent<BaseUnit>();
            enemy.TakeDamage(_unit.Stats.BaseAttackPower + _weaponDmg);
        }

        _canDealDamage = false;
    }
}