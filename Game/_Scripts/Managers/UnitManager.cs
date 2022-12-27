using System.Collections.Generic;
using UnityEngine;

public class UnitManager : StaticInstance<UnitManager>
{
    private Dictionary<int, BaseUnit> _spawnedEnemies = new Dictionary<int, BaseUnit>();
    private int _enemyCount;
    private BaseUnit _playerInstance;
    
    public Transform SpawnPlayer(Transform transform, int level = 1) {
        ScriptableUnitBase playerScriptable = ResourceSystem.Instance.GetPlayer(level);
        GameObject spawnedGO = SpawnUnit(playerScriptable, transform.position, transform.rotation);
        BaseUnit player = spawnedGO.GetComponentInChildren<BaseUnit>();
        player.SetStats(playerScriptable.BaseStats);

        return spawnedGO.transform;
    }

    public void SpawnEnemy(Transform transform, EnemyType type) {
        ScriptableUnitBase enemyScriptable = ResourceSystem.Instance.GetEnemy(type);
        GameObject spawnedGO = SpawnUnit(enemyScriptable, transform.position, transform.rotation);
        
        BaseUnit enemy = spawnedGO.GetComponentInChildren<BaseUnit>();
        enemy.SetId(_enemyCount);
        enemy.SetStats(enemyScriptable.BaseStats);
        _spawnedEnemies.Add(_enemyCount, enemy);
        _enemyCount++;
    }

    private GameObject SpawnUnit(ScriptableUnitBase unitScriptable, Vector3 pos, Quaternion rotation) {
        //TODO: STATS SHOULD BE SET HERE
       GameObject spawned = Instantiate(unitScriptable.Prefab, pos, rotation, transform);

       return spawned;
    }

    public void DespawnEnemy(int id, GameObject obj) {
        _spawnedEnemies.Remove(id);
        Destroy(obj);
    }
}
