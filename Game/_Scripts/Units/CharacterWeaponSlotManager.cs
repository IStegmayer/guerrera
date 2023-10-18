using UnityEngine;

namespace Dzajna {
public class CharacterWeaponSlotManager : MonoBehaviour {
    [Header("Weapons")] [SerializeField] protected WeaponItem rightHandWeapon;
    [SerializeField] protected WeaponItem leftHandWeapon;
    [SerializeField] protected WeaponItem unarmedWeapon;

    protected WeaponHolderSlot rightHandSlot;
    protected WeaponHolderSlot leftHandSlot;
    protected WeaponHolderSlot backSlotRight;
    protected WeaponHolderSlot backSlotLeft;

    protected CharacterManager character;
    protected bool isHyperArmored;

    protected virtual void Awake() => character = GetComponent<CharacterManager>();

    public DamageCollider RightDamageCollider { get; protected set; }
    public DamageCollider LeftDamageCollider { get; protected set; }

    protected virtual void LoadWeaponHolderSlots() {
        WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
        foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots) {
            if (weaponSlot.isLeftHandSlot) leftHandSlot = weaponSlot;
            else if (weaponSlot.isRightHandSlot) rightHandSlot = weaponSlot;
            else if (weaponSlot.isBackSlotLeft) backSlotLeft = weaponSlot;
            else if (weaponSlot.isBackSlotRight) backSlotRight = weaponSlot;
        }
    }

    public virtual void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft) {
        WeaponItem weaponToEquip = weaponItem != null ? weaponItem : unarmedWeapon;
        CharacterAnimatorManager charAnim = character.CharacterAnimatorManager;

        if (isLeft) {
            leftHandSlot.CurrentWeaponItem = weaponToEquip;
            leftHandSlot.LoadWeaponModel(weaponToEquip);
            LoadLeftWeaponDamageCollider();
            charAnim.PlayTargetAnimation(charAnim.LeftArmIdleHash, false, true);
        }
        else {
            if (InputHandler.Instance.TwoHandFlag) {
                charAnim.PlayTargetAnimation(charAnim.TH_IdleHash, false, true);
                leftHandSlot.UnloadWeaponAndDestroy();
            }
            else {
                charAnim.PlayTargetAnimation(charAnim.RightArmIdleHash, false, true);
            }

            rightHandSlot.CurrentWeaponItem = weaponToEquip;
            rightHandSlot.LoadWeaponModel(weaponToEquip);
            LoadRightWeaponDamageCollider();
            charAnim.Anim.runtimeAnimatorController = weaponItem.WeaponController;
        }
    }

    public virtual void LoadWeaponsOnBackSlots(WeaponItem leftWeapon, WeaponItem rightWeapon) {
        backSlotLeft.LoadWeaponModel(leftWeapon);
        backSlotRight.LoadWeaponModel(rightWeapon);
    }    
    public virtual void UnloadBackSlots() {
        backSlotLeft.UnloadWeaponAndDestroy();
        backSlotRight.UnloadWeaponAndDestroy();
    }

    protected virtual void LoadLeftWeaponDamageCollider() {
        WeaponItem weapon = leftHandSlot.CurrentWeaponItem;
        GameObject weaponModel = leftHandSlot.CurrentWeaponModel;

        LeftDamageCollider = weaponModel.GetComponentInChildren<DamageCollider>();
        LeftDamageCollider.characterManager = GetComponentInParent<CharacterManager>();
        LeftDamageCollider.PhysicalDamage = weapon.BaseDamage;
        LeftDamageCollider.PoiseBreak = weapon.PoiseBreak;
        LeftDamageCollider.OffensivePoiseBonus = weapon.OffensivePoiseBonus;

        WeaponFX weaponFX = weaponModel.GetComponentInChildren<WeaponFX>();
        if (weaponFX != null) character.CharacterEffectsManager.LeftWeaponFX = weaponFX;
    }

    protected virtual void LoadRightWeaponDamageCollider() {
        WeaponItem weapon = rightHandSlot.CurrentWeaponItem;
        GameObject weaponModel = rightHandSlot.CurrentWeaponModel;

        RightDamageCollider = weaponModel.GetComponentInChildren<DamageCollider>();
        RightDamageCollider.characterManager = GetComponentInParent<CharacterManager>();
        RightDamageCollider.PhysicalDamage = weapon.BaseDamage;
        RightDamageCollider.PoiseBreak = weapon.PoiseBreak;
        RightDamageCollider.OffensivePoiseBonus = weapon.OffensivePoiseBonus;

        WeaponFX weaponFX = weaponModel.GetComponentInChildren<WeaponFX>();
        if (weaponFX != null) character.CharacterEffectsManager.RightWeaponFX = weaponFX;
    }

    public virtual void OpenDamageCollider() {
        if (character.IsUsingLeftHand) LeftDamageCollider.EnableDamageCollider();
        else if (character.IsUsingRightHand) RightDamageCollider.EnableDamageCollider();
    }

    public virtual void CloseDamageCollider() {
        if (LeftDamageCollider != null) LeftDamageCollider.DisableDamageCollider();
        if (RightDamageCollider != null) RightDamageCollider.DisableDamageCollider();
    }

    public virtual void GrantHyperArmor() {
        if (isHyperArmored) return;
        
        WeaponItem weapon = character.CharacterInventoryManager.CurrentItemBeingUsed as WeaponItem;
        // TODO: this is not good
        if (weapon != null) character.CharacterStatsManager.TotalPoise += weapon.OffensivePoiseBonus;
        isHyperArmored = true;
    }

    public virtual void RemoveHyperArmor() {
        if (!isHyperArmored) return;
        
        WeaponItem weapon = character.CharacterInventoryManager.CurrentItemBeingUsed as WeaponItem;
        if (weapon != null) character.CharacterStatsManager.TotalPoise -= weapon.OffensivePoiseBonus;
        isHyperArmored = false;
    }

    public Transform GetRightHandTransform() => rightHandSlot.transform;
}
}