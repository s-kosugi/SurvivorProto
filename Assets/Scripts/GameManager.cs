using UnityEngine;

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

    public static GameManager Instance { get; private set; }

    private GameState state = GameState.Init;
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
        EnemyManager.Instance?.ClearAllEnemies();
        WaveEventManager.Instance?.ResetWaveState();
        BulletManager.Instance?.ClearAllBullets();
        WaveController.Instance?.ResetWave();
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
    }

    public void EndGame()
    {
        if (state == GameState.GameOver) return;

        Time.timeScale = 0f;
        state = GameState.GameOver;
        gameOverUI.Show(score);
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
