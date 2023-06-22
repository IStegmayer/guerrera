using UnityEngine;

namespace Dzajna {
[CreateAssetMenu(menuName = "Item Actions/Weapon Art Action")]
public class WeaponArtAction : ItemAction {
    public override void PerformAction(CharacterManager character) {
        Debug.Log("This should be a weapon art");
    }
}
}