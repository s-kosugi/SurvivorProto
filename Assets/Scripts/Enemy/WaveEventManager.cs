using UnityEngine;

public class WaveEventManager : MonoBehaviour
{
    public static WaveEventManager Instance { get; private set; }

    public enum WaveState
    {
        Normal,
        WaveStart,
        WaveInProgress,
        WaveClear
    }

    [Header("Wave Settings")]
    [SerializeField] private float waveIntervalSeconds = 120f;
    [SerializeField] private float spawnReductionRate = 0.7f;
    [SerializeField] private float waveClearWaitSeconds = 1.5f;

    [Header("Boss Settings")]
    [SerializeField] private MiniBossBase[] miniBossPrefabs;
    [SerializeField] private float miniBossSpawnDistanceMin = 5f;
    [SerializeField] private float miniBossSpawnDistanceMax = 10f;

    [Header("Rewards")]
    [SerializeField] private int hpRecoveryAmount = 100;

    [Header("References")]
    [SerializeField] private WaveEventUIController ui;
    [SerializeField] private EnemySpawner enemySpawner; 

    private WaveState state = WaveState.Normal;
    private float waveTimer = 0f;
    private MiniBossBase activeBoss;
    private float originalSpawnRate;
    private float waveClearTimer = 0f;
    private PlayerController player;

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
        // EnemySpawnerのスポーン率を保存
        if (enemySpawner == null)
        {
            enemySpawner = FindObjectOfType<EnemySpawner>();
        }

        if (enemySpawner == null)
        {
            Debug.LogError("WaveEventManager: EnemySpawner がシーンに見つかりません。Inspectorで設定してください。");
            return;
        }

        originalSpawnRate = enemySpawner.spawnRate;
        player = FindObjectOfType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("WaveEventManager: プレイヤーがシーンに見つかりません！");
        }
    }
    /// <summary>
    /// WaveController から呼び出す「ミニボス強制開始」
    /// </summary>
    public void ForceStartMiniBoss()
    {
        // 状態初期化
        state = WaveState.WaveStart;
        waveTimer = 0f;
        waveClearTimer = 0f;

        // ミニボス生成
        activeBoss = SpawnMiniBoss();

        // Wave開始UI
        if (ui != null)
        {
            ui.ShowWaveStart();
        }

        // 敵の湧き減衰
        if (enemySpawner != null)
        {
            enemySpawner.spawnRate = originalSpawnRate * spawnReductionRate;
        }
    }


    private void Update()
    {
        switch (state)
        {
            case WaveState.Normal:
                UpdateNormal();
                break;

            case WaveState.WaveStart:
                UpdateWaveStart();
                break;

            case WaveState.WaveInProgress:
                UpdateWaveInProgress();
                break;

            case WaveState.WaveClear:
                UpdateWaveClear();
                break;
        }
    }

    // ======== Normal ========
    private void UpdateNormal()
    {
        //waveTimer += Time.deltaTime;

        //if (waveTimer >= waveIntervalSeconds)
        //{
        //    state = WaveState.WaveStart;
        //}
    }

    // ======== WaveStart ========
    private void UpdateWaveStart()
    {
        //activeBoss = SpawnMiniBoss();

        //if (ui != null)
        //{
        //    ui.ShowWaveStart();
        //}

        //if (enemySpawner != null)
        //{
        //    enemySpawner.spawnRate = originalSpawnRate * spawnReductionRate;
        //}

        state = WaveState.WaveInProgress;
    }

    // ======== WaveInProgress ========
    private void UpdateWaveInProgress()
    {
        if (activeBoss == null || activeBoss.Health.IsDead)
        {
            state = WaveState.WaveClear;
            waveClearTimer = 0f;
        }
    }

    // ======== WaveClear ========
    // WaveClear 内
    private void UpdateWaveClear()
    {
        waveClearTimer += Time.deltaTime;

        if (waveClearTimer < 0.1f)
        {
            player.Health.RecoverHP(hpRecoveryAmount);

            if (ui != null)
            {
                ui.ShowWaveClear();
            }
        }

        if (waveClearTimer >= waveClearWaitSeconds)
        {
            if (enemySpawner != null)
            {
                enemySpawner.spawnRate = originalSpawnRate;
            }

            waveTimer = 0f;
            state = WaveState.Normal;
        }
        BulletManager.Instance.ClearAllBullets();
    }


    // ======== MiniBoss Spawn ========
    private MiniBossBase SpawnMiniBoss()
    {
        Transform playerTransform = player.transform;

        float distance = Random.Range(miniBossSpawnDistanceMin, miniBossSpawnDistanceMax);
        float angle = Random.Range(0f, 360f);
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;

        Vector3 spawnPos = playerTransform.position + (Vector3)offset;

        // ミニボスプレハブ選択（ランダム）
        MiniBossBase bossPrefab = miniBossPrefabs[Random.Range(0, miniBossPrefabs.Length)];

        // 実体生成
        MiniBossBase boss = Instantiate(bossPrefab, spawnPos, Quaternion.identity);

        // ★ HPコンポーネント取得と死亡イベント登録 ★
        boss.Health.OnBossDead += OnMiniBossDead;

        return boss;
    }

    // ボス死亡時コールバック
    private void OnMiniBossDead(MiniBossHealth health)
    {
        // activeBossと一致しているなら破棄
        if (activeBoss != null && activeBoss.Health == health)
        {
            activeBoss = null;
        }
    }
    public void ResetWaveState()
    {
        // Wave中状態を完全初期化
        state = WaveState.Normal;
        waveTimer = 0f;

        // ミニボスの参照クリア
        activeBoss = null;

        // スポーンレートを元に戻す
        if (enemySpawner != null)
        {
            enemySpawner.spawnRate = 1f;
        }

        // UIを消す
        if (ui != null)
        {
            ui.HideAll();
        }

        Debug.Log("[WaveEventManager] ResetWaveState called.");
    }

}
