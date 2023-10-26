using Dzajna;
using UnityEngine;

public class BossTrigger : MonoBehaviour {
    [SerializeField] private EnemyBossManager boss;
    private void OnTriggerEnter(Collider other) {
        FindObjectOfType<EnemyBossManager>().ShowBossHealthBar();
        Destroy(this, 0.1f);
    }
}
