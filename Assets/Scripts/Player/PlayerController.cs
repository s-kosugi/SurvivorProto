using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public PlayerModeState ModeState { get; private set; } = PlayerModeState.Light;
    public PlayerHealth Health => health;

    [Header("References")]
    [SerializeField] private PlayerHealth health;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerVisuals visuals; // 見た目を専用クラスに委譲
    [SerializeField] private PlayerModeSwitcher switcher;

    // ========================
    // ステータス
    // ========================
    [Header("ステータス")]
    public float lightMoveSpeed = 5f;
    public float darkMoveSpeed = 7f;

    private float currentMoveSpeed = 5f;
    public float CurrentMoveSpeed => currentMoveSpeed;   // Movement側が参照

    // ========================
    // 状態制御
    // ========================
    private PlayerControls controls;

    private bool isMeleeAttacking = false;

    // ======================================================
    // 初期化
    // ======================================================
    void Awake()
    {
        controls = new PlayerControls();

        if (health != null)
            health.DeathEvent += HandlePlayerDeath;
    }

    void Start()
    {
        ApplyModeStats();
        visuals.ApplyModeVisual(ModeState); // 初期適用
    }

    // ======================================================
    // ステータス反映
    // ======================================================
    private void ApplyModeStats()
    {
        currentMoveSpeed = (ModeState == PlayerModeState.Light)
            ? lightMoveSpeed
            : darkMoveSpeed;
    }

    // ======================================================
    // メレー攻撃開始/終了
    // ======================================================
    public void BeginMeleeAttack()
    {
        isMeleeAttacking = true;
        playerMovement.MoveStop();
    }

    public void EndMeleeAttack()
    {
        isMeleeAttacking = false;
    }
    /// <summary>
    /// モード切替
    /// </summary>
    public void ToggleMode()
    {
        ModeState = (ModeState == PlayerModeState.Light)
            ? PlayerModeState.Dark
            : PlayerModeState.Light;
    }
    /// <summary>
    /// 各モードスピード設定
    /// </summary>
    public void ApplyModeStatsFromOutside()
    {
        currentMoveSpeed = (ModeState == PlayerModeState.Light)
            ? lightMoveSpeed
            : darkMoveSpeed;
    }
    /// <summary>
    /// 行動できるかどうか？
    /// </summary>
    private bool CanAction()
    {
        if (switcher.IsSwitching) return false;
        if (isMeleeAttacking) return false;

        if (GameManager.Instance == null ||
            GameManager.Instance.State != GameState.Playing)
            return false;

        return true;
    }

    /// <summary>
    /// 攻撃できるかどうか
    /// </summary>
    public bool CanAttack()
    {
        return CanAction();
    }

    /// <summary>
    /// 移動できるかどか
    /// </summary>
    public bool CanMove()
    {
        return CanAction();
    }
    /// <summary>
    /// モード切替できるかどうか
    /// </summary>
    public bool CanSwitchMode()
    {
        // モード切替できるか
        return CanAction();
    }

    // ======================================================
    // 死亡処理
    // ======================================================
    private void HandlePlayerDeath()
    {
        EndMeleeAttack();
        playerMovement.MoveStop();
        visuals.ClearAllEffects();
    }
}
