using UnityEngine;

public class WaveEventManager : MonoBehaviour
{
    public static WaveEventManager Instance { get; private set; }

    // ===== MiniBoss =====
    [Header("MiniBoss Settings")]
    [SerializeField] private MiniBossBase[] miniBossPrefabs;

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
    public void ForceStartMiniBoss(MiniBossConfig config)
    {
        ui?.ShowWaveStart();
        SpawnMiniBoss(config);
    }
    private void SpawnMiniBoss(MiniBossConfig config)
    {
        for (int i = 0; i < config.spawnCount; i++)
        {
            float distance = Random.Range(config.spawnDistanceMin, config.spawnDistanceMax);
            float angle = Random.Range(0f, 360f);
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;

            Vector3 spawnPos = player.transform.position + (Vector3)offset;

            MiniBossBase prefab = config.miniBossPrefabs[Random.Range(0, config.miniBossPrefabs.Length)];

            MiniBossBase boss = Instantiate(prefab, spawnPos, Quaternion.identity);

            // ★ ここを追加！
            activeBoss = boss;

            boss.Health.OnBossDead += OnMiniBossDead;
        }
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
