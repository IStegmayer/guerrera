using UnityEngine;

public class UnitManager : StaticInstance<UnitManager>
{
    
    public Transform SpawnPlayer(Vector3 pos, int level = 1) {
        ScriptableUnitBase playerScriptable = ResourceSystem.Instance.GetPlayer(level);
        return SpawnUnit(playerScriptable, pos);
    }

    public void SpawnEnemy(Vector3 pos, EnemyType type) {
        ScriptableUnitBase enemyScriptable = ResourceSystem.Instance.GetEnemy(type);
        SpawnUnit(enemyScriptable, pos);
    }

    private Transform SpawnUnit(ScriptableUnitBase unitScriptable,Vector3 pos) {
        //UnitBase, not var
       var spawned = Instantiate(unitScriptable.Prefab, pos, Quaternion.identity, transform);

       return spawned.transform;
       // Apply possible modifications here such as potion boosts, team synergies, etc
       // var stats = unitScriptable.BaseStats;
       // stats.Health += 20;     
       //
       // spawned.SetStats(stats);
    }
}
