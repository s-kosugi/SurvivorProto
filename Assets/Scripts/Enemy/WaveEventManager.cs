using UnityEngine;

public class WaveEventManager : MonoBehaviour
{
    public static WaveEventManager Instance { get; private set; }

    // ===== MiniBoss =====
    [Header("MiniBoss Settings")]
    [SerializeField] private MiniBossBase[] miniBossPrefabs;
    [SerializeField] private float miniBossSpawnDistanceMin = 5f;
    [SerializeField] private float miniBossSpawnDistanceMax = 10f;

    // ===== Rewards =====
    [Header("Rewards")]
    [SerializeField] private int hpRecoveryAmount = 100;

    // ===== UI =====
    [Header("UI")]
    [SerializeField] private WaveEventUIController ui;

    // ===== Internal =====
    private MiniBossBase activeBoss;
    private PlayerController player;

    // ===== Callbacks =====
    public System.Action OnMiniBossCleared;   // WaveController へ通知

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        if (player == null)
            Debug.LogError("[WaveEventManager] Player not found!");
    }

    // ============================================================
    //  Force Start MiniBoss (called from WaveController)
    // ============================================================
    public void ForceStartMiniBoss()
    {
        // UI: Wave Start
        ui?.ShowWaveStart();

        // Spawn MiniBoss
        SpawnMiniBoss();
    }

    private void SpawnMiniBoss()
    {
        if (player == null || miniBossPrefabs.Length == 0)
        {
            Debug.LogError("[WaveEventManager] SpawnMiniBoss failed.");
            return;
        }

        float distance = Random.Range(miniBossSpawnDistanceMin, miniBossSpawnDistanceMax);
        float angle = Random.Range(0f, 360f);
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;

        Vector3 spawnPos = player.transform.position + (Vector3)offset;

        // Prefab選択
        MiniBossBase prefab = miniBossPrefabs[Random.Range(0, miniBossPrefabs.Length)];

        // 実体生成
        activeBoss = Instantiate(prefab, spawnPos, Quaternion.identity);

        // 死亡イベント登録
        activeBoss.Health.OnBossDead += OnMiniBossDead;
    }

    // ============================================================
    //  MiniBoss Dead
    // ============================================================
    private void OnMiniBossDead(MiniBossHealth health)
    {
        if (activeBoss != null && activeBoss.Health == health)
        {
            activeBoss = null;

            // 回復
            player?.Health.RecoverHP(hpRecoveryAmount);

            // UI
            ui?.ShowWaveClear();

            // 弾クリア
            BulletManager.Instance?.ClearAllBullets();

            // WaveControllerへ通知
            OnMiniBossCleared?.Invoke();
        }
    }

    // ============================================================
    //  Reset for RestartGame
    // ============================================================
    public void ResetWaveState()
    {
        activeBoss = null;

        ui?.HideAll();

        Debug.Log("[WaveEventManager] ResetWaveState done.");
    }
}
