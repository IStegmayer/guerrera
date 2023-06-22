using UnityEngine;

namespace Dzajna {
[CreateAssetMenu(menuName = "Item Actions/Charged Attack Action")]
public class ChargedAttackAction : ItemAction {
    public override void PerformAction(CharacterManager character) {
        Debug.Log("This should be a charged attack");
    }
}
}