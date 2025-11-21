using System.Collections;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    public static WaveController Instance { get; private set; }

    [Header("Wave Settings")]
    public WaveConfig[] waveConfigs;     // WaveConfigへの参照（Wave1,2,3）
    public float waveDuration = 10f;     // 各Waveの時間（テスト用10秒）
    public float miniBossTime = 30f;     // MiniBoss呼び出し時間（Wave3終了後）

    [Header("Spawners")]
    public EnemySpawner[] spawners;      // 複数スポナー

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

    private void Update()
    {
        timer += Time.deltaTime;

        // Wave切り替え 0 → 1 → 2
        if (currentWave < waveConfigs.Length)
        {
            float startTime = currentWave * waveDuration;
            float endTime = (currentWave + 1) * waveDuration;

            if (timer >= startTime && timer < endTime)
            {
                StartWave(currentWave);
            }

            // Waveの終了タイミングを超えたら次へ（自動進行）
            if (timer >= endTime)
            {
                currentWave++;
            }
        }

        // MiniBoss 呼び出しタイミング
        if (!miniBossCalled && timer >= miniBossTime)
        {
            CallMiniBoss();
        }
    }

    /// <summary>
    /// 指定Waveの開始処理（RunWaveの実行）
    /// </summary>
    private void StartWave(int waveIndex)
    {
        if (waveRoutine == null)
        {
            if (waveIndex >= 0 && waveIndex < waveConfigs.Length)
            {
                waveRoutine = StartCoroutine(RunWave(waveConfigs[waveIndex]));
            }
        }
    }

    /// <summary>
    /// WaveConfig に従って敵をスポーン
    /// </summary>
    private IEnumerator RunWave(WaveConfig config)
    {
        foreach (var enemySet in config.enemies)
        {
            for (int i = 0; i < enemySet.count; i++)
            {
                // ランダムなスポナー選択
                EnemySpawner spawner = spawners[Random.Range(0, spawners.Length)];

                // 敵出現
                spawner.SpawnEnemy(enemySet.prefab);

                yield return new WaitForSeconds(enemySet.interval);
            }
        }

        waveRoutine = null;
    }

    /// <summary>
    /// MiniBoss呼び出し（WaveEventManagerへ委譲）
    /// </summary>
    private void CallMiniBoss()
    {
        miniBossCalled = true;

        // 雑魚出現OFF
        foreach (var s in spawners)
        {
            s.autoSpawn = false;
        }

        if (WaveEventManager.Instance != null)
        {
            WaveEventManager.Instance.ForceStartMiniBoss();
        }
    }

    /// <summary>
    /// リスタート時に必要な初期化
    /// </summary>
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

        // 自動湧きをOFFに戻す
        foreach (var s in spawners)
        {
            s.autoSpawn = false;
        }
    }
}
