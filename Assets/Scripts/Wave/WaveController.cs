using System.Collections;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    public static WaveController Instance { get; private set; }

    [Header("Wave Profile (Normal / Debug / BossOnly)")]
    [SerializeField] private WaveProfile waveProfile;

    // Profile適用後に使う内部変数（Inspectorには見せない）
    [HideInInspector] public WaveConfig[] waveConfigs;
    [HideInInspector] public float waveDuration;
    [HideInInspector] public float miniBossTime;
    [HideInInspector] public MiniBossConfig[] miniBossConfigs;

    [Header("Spawners")]
    public EnemySpawner[] spawners;

    private float timer = 0f;
    private int currentWave = 0;
    private bool miniBossCalled = false;
    private int currentMiniBossIndex = 0;
    private float nextMiniBossTime;

    private Coroutine waveRoutine;

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
        ApplyWaveProfile();                 // WaveProfileの内容を反映
        nextMiniBossTime = miniBossTime;
    }

    private void OnEnable()
    {
        if (WaveEventManager.Instance != null)
            WaveEventManager.Instance.OnMiniBossCleared += HandleMiniBossCleared;
    }

    private void OnDisable()
    {
        if (WaveEventManager.Instance != null)
            WaveEventManager.Instance.OnMiniBossCleared -= HandleMiniBossCleared;
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
            return;

        timer += Time.deltaTime;

        // ===== Wave進行 =====
        if (!miniBossCalled && currentWave < waveConfigs.Length)
        {
            float startTime = currentWave * waveDuration;
            float endTime = (currentWave + 1) * waveDuration;

            if (timer >= startTime && timer < endTime)
            {
                StartWave(currentWave);
            }

            // MiniBossタイムを越えたらBoss発生
            if (timer >= nextMiniBossTime)
            {
                CallMiniBoss();
            }

            // WaveDurationによる自然切替
            if (timer >= endTime && !miniBossCalled)
            {
                currentWave++;
            }
        }
    }


    private void ApplyWaveProfile()
    {
        waveConfigs = waveProfile.waveConfigs;
        waveDuration = waveProfile.timing.waveDuration;
        miniBossTime = waveProfile.timing.miniBossTime;
        miniBossConfigs = waveProfile.miniBossConfigSet.miniBosses;
    }

    private void StartWave(int waveIndex)
    {
        if (waveRoutine == null && waveIndex < waveConfigs.Length)
        {
            waveRoutine = StartCoroutine(RunWave(waveConfigs[waveIndex]));
        }
    }

    private IEnumerator RunWave(WaveConfig config)
    {
        foreach (var enemySet in config.enemies)
        {
            for (int i = 0; i < enemySet.count; i++)
            {
                spawners[0].SpawnEnemy(enemySet.enemyId);
                yield return new WaitForSeconds(enemySet.interval);
            }
        }

        waveRoutine = null;
    }

    // ============================================================
    //  MiniBoss呼び出し
    // ============================================================
    private void CallMiniBoss()
    {
        if (miniBossCalled) return;

        miniBossCalled = true;

        // 雑魚の自動湧きOFF
        foreach (var s in spawners)
            s.autoSpawn = false;

        // ボスを順番どおり呼び出し
        WaveEventManager.Instance.ForceStartMiniBoss(miniBossConfigs[currentMiniBossIndex]);

        // 次のボスのインデックス（ループする）
        currentMiniBossIndex = (currentMiniBossIndex + 1) % miniBossConfigs.Length;

        // 次のボスまでの予定時間を追加
        nextMiniBossTime += miniBossTime;
    }


    // ============================================================
    //  MiniBoss撃破 → 次Waveへ
    // ============================================================
    private void HandleMiniBossCleared()
    {
        // 次のWaveへ進める
        currentWave++;

        // Waveタイマーを次のWaveの開始位置に補正
        timer = currentWave * waveDuration;

        // 次のMiniBoss予定時間を再セット
        nextMiniBossTime = timer + miniBossTime;

        // Boss中フラグ解除
        miniBossCalled = false;

        // 雑魚スポナー再開
        foreach (var s in spawners)
            s.autoSpawn = true;

    }

    // ============================================================
    //  Reset on Restart
    // ============================================================
    public void ResetWave()
    {
        ApplyWaveProfile(); // リセット時もProfileの値再反映

        timer = 0f;
        currentWave = 0;

        // 次のMiniBoss発生予定時間をリセット
        nextMiniBossTime = miniBossTime;
        // MiniBossの出現インデックス（ループ）を初期化
        currentMiniBossIndex = 0;
        // Boss中フラグ解除
        miniBossCalled = false;
        // 自動スポーンを確実にONに戻す
        foreach (var s in spawners)
            s.autoSpawn = true;
        // WaveRoutine停止
        if (waveRoutine != null)
        {
            StopCoroutine(waveRoutine);
            waveRoutine = null;
        }
        // WaveEventManager側のリセット処理
        WaveEventManager.Instance?.ResetWaveState();
        // イベント処理安全対策
        WaveEventManager.Instance.OnMiniBossCleared -= HandleMiniBossCleared;
        WaveEventManager.Instance.OnMiniBossCleared += HandleMiniBossCleared;
    }
}
