using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyPrefabDatabase database;

    [Header("Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float baseSpawnInterval = 2f; 
    [SerializeField] private float spawnRadius = 8f;
    [SerializeField] private int maxEnemies = 20;

    [Header("Wave Control")]
    public float spawnRate = 1f;         // 自動湧き時のスケール
    public bool autoSpawn = false;        // ← 新規：自動スポーンON/OFF

    private float timer;
    private Transform player;

    private void Start()
    {
        player = PlayerManager.Instance.MainPlayer.transform;

        if (player == null)
        {
            Debug.LogError("[EnemySpawner] Player not found (tag Player missing?)");
        }
    }

    private void Update()
    {
        // ゲームがプレイ状態でなければできない
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
            return;
        // 自動スポーンをオフにしている場合は何もしない
        if (!autoSpawn) return;

        float currentSpawnInterval = baseSpawnInterval / Mathf.Max(spawnRate, 0.01f);

        timer += Time.deltaTime;

        if (timer >= currentSpawnInterval)
        {
            timer = 0f;
            TrySpawnEnemy();
        }
    }

    /// <summary>
    /// 自動スポーンで使用（内部用）
    /// </summary>
    private void TrySpawnEnemy()
    {
        if (EnemyManager.Instance == null)
        {
            Debug.LogWarning("[EnemySpawner] EnemyManager not found!");
            return;
        }

        if (EnemyManager.Instance.TotalEnemyCount >= maxEnemies)
            return;

        if (player == null || enemyPrefab == null) return;

        Vector2 randomDir = Random.insideUnitCircle.normalized * spawnRadius;
        Vector2 spawnPos = (Vector2)player.position + randomDir;

        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, transform);
        EnemyManager.Instance.RegisterEnemy(enemy);
    }

    /// <summary>
    /// Wave制御用：手動で敵をスポーンさせる
    /// </summary>
    public void SpawnEnemy(EnemyID id)
    {
        var prefab = database.GetPrefab(id);

        if (prefab == null)
            return;
        Vector2 randomDir = Random.insideUnitCircle.normalized * spawnRadius;
        Vector2 spawnPos = (Vector2)player.position + randomDir;
        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity, transform);
        EnemyManager.Instance.RegisterEnemy(enemy);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0.3f, 0.3f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
