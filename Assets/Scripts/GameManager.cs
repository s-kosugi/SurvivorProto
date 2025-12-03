using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Init,
    Playing,
    GameOver
}

public class GameManager : MonoBehaviour
{
    [SerializeField] PlayerCore core;
    [SerializeField] ScoreUI scoreUI;
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private WaveEventUIController waveEventUIController;

    public static GameManager Instance { get; private set; }

    private GameState state = GameState.Init;
    public GameState State => state;
    private int score = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        state = GameState.Init;
        Time.timeScale = 1f;

        // BGM再生
        SoundManager.Instance.PlayBGM("Stage1");
    }

    void Start()
    {
        gameOverUI.Hide();
        gameOverUI.BindRestart(RestartGame);

        StartGame();
    }

    public void StartGame()
    {
        // 敵・弾・アイテム・エフェクト初期化
        WaveEventManager.Instance?.ResetWaveState();
        WaveController.Instance?.ResetWave();
        EnemyManager.Instance?.ClearAllEnemies();
        BulletManager.Instance?.ClearAllBullets();
        ItemManager.Instance?.ClearAll();
        EffectLibrary.Instance?.ClearAllEffects();

        // スコア初期化
        score = 0;
        scoreUI.UpdateScore(score);

        // 状態切り替え
        state = GameState.Playing;
        Time.timeScale = 1f;

        // プレイヤー初期化
        core.transform.position = Vector3.zero;
        core.health.ResetHealth();

        gameOverUI.Hide();
        waveEventUIController.ResetUI();
    }

    public void EndGame()
    {
        if (state == GameState.GameOver) return;

        state = GameState.GameOver;
        Time.timeScale = 1f; // ← DefeatSceneは動くので1に戻しておく

        // --- ★ SessionData に結果を保存する ---
        SessionData.LightLevel = core.expCollector.lightLevel;
        SessionData.DarkLevel = core.expCollector.darkLevel;

        SessionData.LightExp = core.expCollector.lightExp;
        SessionData.DarkExp = core.expCollector.darkExp;

        SessionData.Score = score;

        // βで使う敵ID（今は空でもOK）
        //SessionData.KilledBy = core.health.LastDamageSourceID;

        // --- ★ DefeatScene へ遷移する ---
        SceneManager.LoadScene("DefeatScene");
    }

    public void RestartGame()
    {
        Debug.Log("Restart executed by UI!");
        Time.timeScale = 1f;
        StartGame();
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreUI.UpdateScore(score);
    }
}
