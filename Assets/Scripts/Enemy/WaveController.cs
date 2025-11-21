using System.Collections;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    public static WaveController Instance { get; private set; }

    [Header("Wave Settings")]
    public WaveConfig[] waveConfigs;
    public float waveDuration = 10f;      // Wave切替時間
    public float miniBossTime = 30f;      // MiniBoss発生時間

    [Header("Spawners")]
    public EnemySpawner[] spawners;       // 1つだけでOK（相対座標スポーン）

    private float timer = 0f;
    private int currentWave = 0;
    private bool miniBossCalled = false;

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
        // MiniBoss撃破イベント受信
        if (WaveEventManager.Instance != null)
        {
            WaveEventManager.Instance.OnMiniBossCleared += HandleMiniBossCleared;
        }
    }

    private void Update()
    {
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
            if (timer >= miniBossTime)
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

    // ============================================================
    //  Wave開始
    // ============================================================
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
                spawners[0].SpawnEnemy(enemySet.prefab);
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

        // 雑魚の自動湧きは使用しないが念のためOFF
        foreach (var s in spawners)
        {
            s.autoSpawn = false;
        }

        WaveEventManager.Instance?.ForceStartMiniBoss();
    }

    // ============================================================
    //  MiniBoss撃破 → 次Waveへ
    // ============================================================
    private void HandleMiniBossCleared()
    {
        currentWave++;

        // タイマーを次Waveへ補正
        timer = currentWave * waveDuration;

        // 再びBoss呼び出し可能に
        miniBossCalled = false;

        Debug.Log($"[WaveController] MiniBoss cleared → Wave {currentWave + 1}");
    }

    // ============================================================
    //  Reset on Restart
    // ============================================================
    public void ResetWave()
    {
        timer = 0f;
        currentWave = 0;
        miniBossCalled = false;

        if (waveRoutine != null)
        {
            StopCoroutine(waveRoutine);
            waveRoutine = null;
        }

        WaveEventManager.Instance?.ResetWaveState();

        Debug.Log("[WaveController] ResetWave done.");
    }
}
