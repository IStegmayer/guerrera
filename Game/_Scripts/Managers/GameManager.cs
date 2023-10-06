using System;
using Cinemachine;
using Dzajna;
using UnityEngine;

public class GameManager : StaticInstance<GameManager>
{
    [SerializeField] private Transform playerSpawn;
    [SerializeField] private Transform enemySpawn;
    [SerializeField] private Transform enemySpawn2;
    [SerializeField] private Transform enemySpawn3;
    [SerializeField] private Transform enemySpawn4;
    [SerializeField] private Transform enemySpawn5;
    [SerializeField] private Transform mainCamera;
    [SerializeField] private UIManager uiManager;
    public static event Action<GameState> OnBeforeStateChanged;
    public static event Action<GameState> OnAfterStateChanged;

    public GameState CurrentGameState { get; private set; }

    // Kick the game off with the first state   
    void Start() => ChangeState(GameState.Starting);
    public void ChangeState(GameState newState) {
        OnBeforeStateChanged?.Invoke(newState);

        CurrentGameState = newState;
        switch (newState) {
            case GameState.Starting:
                HandleStarting();
                break;
            case GameState.SpawningHeroes:
                HandleSpawningHeroes();
                break;
            case GameState.SpawningEnemies:
                HandleSpawningEnemies();
                break;
            case GameState.HeroTurn:
                HandleHeroTurn();
                break;
            case GameState.Win:
                HandleWin();
                break;
            case GameState.Lose:
                HandleLose();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnAfterStateChanged?.Invoke(newState);
        
        // Debug.Log($"New state: {newState}");
    }

    private void HandleStarting() {
        // Do some start setup, could be environment, cinematics etc

        // Eventually call ChangeState again with your next state
        
        ChangeState(GameState.SpawningHeroes);
    }

    private void HandleSpawningHeroes() {
        // Transform playerInstance = UnitManager.Instance.SpawnPlayer(playerSpawn);
        // PlayerCamera.Instance.SetTarget(playerInstance.GetComponent<PlayerManager>());
        // uiManager.CharacterInventoryManager = playerInstance.GetComponent<CharacterInventoryManager>();
        ChangeState(GameState.SpawningEnemies);
    }

    private void HandleWin() { }
    private void HandleLose() { }

    private void HandleSpawningEnemies() {
        UnitManager.Instance.SpawnEnemy(enemySpawn, EnemyType.FemaleWarriorGoblin);
        UnitManager.Instance.SpawnEnemy(enemySpawn2, EnemyType.FemaleWarriorGoblin);
        UnitManager.Instance.SpawnEnemy(enemySpawn3, EnemyType.FemaleWarriorGoblin);
        UnitManager.Instance.SpawnEnemy(enemySpawn4, EnemyType.Shaman);
        UnitManager.Instance.SpawnEnemy(enemySpawn5, EnemyType.Shaman);
        
        ChangeState(GameState.HeroTurn);
    }

    private void HandleHeroTurn() {
        // If you're making a turn based game, this could show the turn menu, highlight available units etc
        
        // Keep track of how many units need to make a move, once they've all finished, change the state. This could
        // be monitored in the unit manager or the units themselves.
    }
}
    /// <summary>
    /// This is obviously an example and I have no idea what kind of game you're making.
    /// You can use a similar manager for controlling your menu states or dynamic-cinematics, etc
    /// </summary>
    [Serializable]
    public enum GameState
    {
        Starting = 0,
        SpawningHeroes = 1,
        SpawningEnemies = 2,
        HeroTurn = 3,
        Win = 4,
        Lose = 5,
    }
