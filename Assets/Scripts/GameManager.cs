using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum GameState
{
    Init,
    Playing,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("UI References")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text resultScoreText;
    [SerializeField] private Button restartButton;
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
    }

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        StartGame();
    }

    /// <summary>
    /// スタート時ゲーム初期化
    /// </summary>
    public void StartGame()
    {
        // 敵を全消去
        if (EnemyManager.Instance != null){
            EnemyManager.Instance.ClearAllEnemies();
        }

        // スコア初期化
        score = 0;
        UpdateScoreUI();

        // 状態切り替え
        state = GameState.Playing;
        Time.timeScale = 1f;

        // プレイヤーを初期位置に戻す
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = Vector3.zero;
            var hp = player.GetComponent<PlayerHealth>();
            if (hp != null)
            {
                // HP再初期化
                hp.ResetHealth();
            }
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
    /// <summary>
    /// ゲームオーバー処理
    /// </summary>
    public void EndGame()
    {
        if (state == GameState.GameOver) return;
        state = GameState.GameOver;
        Time.timeScale = 0f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        if (resultScoreText != null)
            resultScoreText.text = $"Score: {score}";
    }
    /// <summary>
    /// ゲームリスタート処理
    /// </summary>
    public void RestartGame()
    {
        Debug.Log("Restart button pressed!");
        Time.timeScale = 1.0f;
        StartGame();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }
}
