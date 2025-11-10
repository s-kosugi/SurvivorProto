using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnRadius = 8f;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        InvokeRepeating(nameof(SpawnEnemy), 1f, spawnInterval);
    }

    void SpawnEnemy()
    {
        if (player == null || enemyPrefab == null) return;

        // プレイヤーの周囲ランダムな位置に出現
        Vector2 randomDir = Random.insideUnitCircle.normalized * spawnRadius;
        Vector2 spawnPos = (Vector2)player.position + randomDir;

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}
