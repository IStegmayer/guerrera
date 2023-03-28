using System;
using UnityEngine;

namespace Dzajna {
public class EnemyBossManager : MonoBehaviour {
    [SerializeField] private string bossName;
    private BossHealthBar bossHealthBar;
    private EnemyStats enemyStats;

    private void Awake() {
        bossHealthBar = FindObjectOfType<BossHealthBar>(true);
        enemyStats = GetComponent<EnemyStats>();
    }

    private void Start() {
        bossHealthBar.ToggleBossHealthBarActivation(true);
        bossHealthBar.SetBossName(bossName);
        bossHealthBar.SetBossMaxHealth(enemyStats.GetMaxHealth());
    }

    public void UpdateBossHealthBar(float _currentHealth) {
        bossHealthBar.SetBossCurrentHealth(_currentHealth);
    }
    //HANDLE SWITCHING PHASE 
    //HANDLE SWITCHING ATTACK PATTERNS
}
}