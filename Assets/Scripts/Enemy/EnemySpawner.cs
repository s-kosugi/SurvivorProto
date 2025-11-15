using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float baseSpawnInterval = 2f; // 基準のスポーン間隔
    [SerializeField] private float spawnRadius = 8f;
    [SerializeField] private int maxEnemies = 20;

    [Header("Wave Control")]
    public float spawnRate = 1f; // WaveEventManagerが変更するスケール値 1=通常

    private float timer;
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player == null)
        {
            Debug.LogError("[EnemySpawner] Player not found (tag Player missing?)");
        }
    }

    private void Update()
    {
        // 実際のスポーン間隔は spawnRate に応じて変化
        float currentSpawnInterval = baseSpawnInterval / Mathf.Max(spawnRate, 0.01f);

        timer += Time.deltaTime;

        if (timer >= currentSpawnInterval)
        {
            timer = 0f;
            TrySpawnEnemy();
        }
    }

    private void TrySpawnEnemy()
    {
        // Manager存在チェック
        if (EnemyManager.Instance == null)
        {
            Debug.LogWarning("[EnemySpawner] EnemyManager not found!");
            return;
        }

        // 上限チェック
        if (EnemyManager.Instance.TotalEnemyCount >= maxEnemies)
            return;

        // プレイヤー位置チェック
        if (player == null || enemyPrefab == null) return;

        // プレイヤー周囲にランダムスポーン
        Vector2 randomDir = Random.insideUnitCircle.normalized * spawnRadius;
        Vector2 spawnPos = (Vector2)player.position + randomDir;

        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, transform);
        EnemyManager.Instance.RegisterEnemy(enemy);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0.3f, 0.3f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
