using System;
using Cinemachine;
using UnityEngine;

public class GameManager : StaticInstance<GameManager>
{
    [SerializeField] private Transform _playerSpawn;
    [SerializeField] private Transform _enemySpawn;
    [SerializeField] private CinemachineFreeLook _mainCamera;
    private void Start() {
        Transform playerInstance = UnitManager.Instance.SpawnPlayer(_playerSpawn.position);
        UnitManager.Instance.SpawnEnemy(_enemySpawn.position, EnemyType.Goblin);

        _mainCamera.Follow = playerInstance;
        _mainCamera.LookAt = playerInstance;
    }
}
