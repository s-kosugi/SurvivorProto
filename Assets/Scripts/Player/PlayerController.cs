using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public PlayerModeState ModeState { get; private set; } = PlayerModeState.Light;
    public PlayerHealth Health => health;

    [SerializeField] private PlayerHealth health;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMovement playerMovement;

    // ========================
    // モード切替設定
    // ========================
    [Header("ModeChange")]
    [SerializeField] private float switchCooldown = 1.0f; 
    [SerializeField] private float switchLag = 0.3f;

    // ========================
    // 見た目用
    // ========================
    [Header("Mode Effects")]
    [SerializeField] private GameObject lightAura;
    [SerializeField] private GameObject darkAura;
    [SerializeField] private GameObject switchEffectPrefab;
    [SerializeField] private GameObject lightFlashPrefab;
    [SerializeField] private GameObject darkFlashPrefab;

    // ========================
    // ステータス
    // ========================
    [Header("ステータス")]
    public float lightMoveSpeed = 5f;
    public float darkMoveSpeed = 7f;

    private float currentMoveSpeed = 5f;
    public float CurrentMoveSpeed => currentMoveSpeed;   // ← Movement側が参照

    // ========================
    // 状態制御
    // ========================
    private PlayerControls controls;
    private List<GameObject> spawnedEffects = new List<GameObject>();

    private bool isMeleeAttacking = false;
    private bool isSwitching = false;
    private float lastSwitchTime = -999f;


    void Awake()
    {
        controls = new PlayerControls();

        if (health != null)
            health.DeathEvent += HandlePlayerDeath;
    }

    void Start()
    {
        ApplyModeStats();
    }

    void OnEnable()
    {
        controls.Enable();
        controls.Player.ElementalSwitch.performed += _ => SwitchMode();
    }

    void OnDisable()
    {
        controls.Player.ElementalSwitch.performed -= _ => SwitchMode();
        controls.Disable();
    }

    // ======================================================
    // モード切替
    // ======================================================
    private void SwitchMode()
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

        // 移動停止
        playerMovement.MoveStop();

        // 状態切替
        ModeState = (ModeState == PlayerModeState.Light)
            ? PlayerModeState.Dark
            : PlayerModeState.Light;

        ApplyModeVisual();
        ApplyModeStats();

        // ラグ
        yield return new WaitForSeconds(switchLag);

        isSwitching = false;
    }

    // ======================================================
    // 見た目変更
    // ======================================================
    private void ApplyModeVisual()
    {
        lightAura.SetActive(ModeState == PlayerModeState.Light);
        darkAura.SetActive(ModeState == PlayerModeState.Dark);

        var fxPrefab = (ModeState == PlayerModeState.Light)
            ? lightFlashPrefab
            : darkFlashPrefab;

        var fx = Instantiate(fxPrefab, transform.position, Quaternion.identity);
        spawnedEffects.Add(fx);
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
    /// <summary>
    /// 移動できるか？
    /// </summary>
    public bool CanMove()
    {
        // 切替中・攻撃中は移動禁止
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
        animator.SetFloat("MoveSpeed", 0f);
        ClearAllEffects();
    }

    public void ClearAllEffects()
    {
        lightAura.SetActive(false);
        darkAura.SetActive(false);

        foreach (var fx in spawnedEffects)
        {
            if (fx != null) Destroy(fx);
        }
        spawnedEffects.Clear();
    }
}
