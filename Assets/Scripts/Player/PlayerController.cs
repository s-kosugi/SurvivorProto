using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public PlayerModeState ModeState { get; private set; } = PlayerModeState.Light;
    public PlayerHealth Health => health;

    [Header("References")]
    [SerializeField] private PlayerHealth health;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerVisuals visuals; // ← 追加：見た目を専用クラスに委譲

    // ========================
    // モード切替設定
    // ========================
    [Header("ModeChange")]
    [SerializeField] private float switchCooldown = 1.0f;
    [SerializeField] private float switchLag = 0.3f;

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
    private bool isSwitching = false;
    private float lastSwitchTime = -999f;


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

    void OnEnable()
    {
        controls.Enable();
        controls.Player.ElementalSwitch.performed += _ => TrySwitchMode();
    }

    void OnDisable()
    {
        controls.Player.ElementalSwitch.performed -= _ => TrySwitchMode();
        controls.Disable();
    }

    // ======================================================
    // モード切替
    // ======================================================
    private void TrySwitchMode()
    {
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
            return;

        if (isMeleeAttacking) return;
        if (Time.time - lastSwitchTime < switchCooldown) return;

        StartCoroutine(DoSwitchMode());
    }

    private IEnumerator DoSwitchMode()
    {
        isSwitching = true;
        lastSwitchTime = Time.time;

        // 移動停止（Movementに委譲）
        playerMovement.MoveStop();

        // 状態切替
        ModeState = (ModeState == PlayerModeState.Light)
            ? PlayerModeState.Dark
            : PlayerModeState.Light;

        // 見た目は Visuals に委譲
        visuals.ApplyModeVisual(ModeState);

        // ステータス反映
        ApplyModeStats();

        // ラグ
        yield return new WaitForSeconds(switchLag);

        isSwitching = false;
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

    // ======================================================
    // 攻撃できるか？
    // ======================================================
    public bool CanAttack()
    {
        if (isSwitching) return false;
        if (isMeleeAttacking) return false;

        if (GameManager.Instance == null ||
            GameManager.Instance.State != GameState.Playing)
            return false;

        return true;
    }

    // ======================================================
    // 移動できるか？
    // ======================================================
    public bool CanMove()
    {
        if (isSwitching) return false;
        if (isMeleeAttacking) return false;

        return GameManager.Instance.State == GameState.Playing;
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
