using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnRadius = 8f;
    [SerializeField] private int maxEnemies = 20;

    private float timer;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            TrySpawnEnemy();
        }
    }
    private void TrySpawnEnemy()
    {
        // 現在の敵数を確認
        if (EnemyManager.Instance == null)
        {
            Debug.LogWarning("[EnemySpawner] EnemyManager not found!");
            return;
        }

        // Manager側の全体敵数チェック
        if (EnemyManager.Instance.TotalEnemyCount >= maxEnemies)
            return;

        // ランダムな位置を決定
        if (player == null || enemyPrefab == null) return;

        // プレイヤーの周囲ランダムな位置に出現
        Vector2 randomDir = Random.insideUnitCircle.normalized * spawnRadius;
        Vector2 spawnPos = (Vector2)player.position + randomDir;

        // Spawner配下に敵を生成する
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, transform);

        // Managerに登録通知
        EnemyManager.Instance.RegisterEnemy(enemy);
    }

    // Editorで範囲確認
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0.3f, 0.3f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
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
