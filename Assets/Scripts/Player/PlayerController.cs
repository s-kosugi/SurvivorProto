using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public PlayerModeState ModeState { get; private set; } = PlayerModeState.Light;
    public PlayerHealth Health => health;
    [SerializeField] private PlayerHealth health;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator animator;
    [Header("ModeChange")]
    [SerializeField] private float switchCooldown = 1.0f; // 切替後のクールダウン
    [SerializeField] private float switchLag = 0.3f;      // 切替中は硬直する（動けない・攻撃できない）

    [Header("Mode Effects")]
    [SerializeField] private GameObject lightAura;
    [SerializeField] private GameObject darkAura;
    [SerializeField] private GameObject switchEffectPrefab;
    [SerializeField] private GameObject lightFlashPrefab;
    [SerializeField] private GameObject darkFlashPrefab;

    [Header("ステータス")]
    public float lightMoveSpeed = 5f;
    public float darkMoveSpeed = 7f;

    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector2 lastMoveDir = Vector2.right;
    private float currentMoveSpeed = 5f;
    private List<GameObject> spawnedEffects = new List<GameObject>();
    public bool IsFacingLeft {get; private set; }
    // 近接攻撃中かどうか
    private bool isMeleeAttacking = false;

    // モード切替用
    private bool isSwitching = false;
    private float lastSwitchTime = -999f;


    void Awake()
    {
        controls = new PlayerControls();
        IsFacingLeft = false;

        if (health != null)
            health.DeathEvent += HandlePlayerDeath;
    }

    private void Start()
    {
        ApplyModeStats();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Player.ElementalSwitch.performed += _ => SwitchMode();
    }

    private void OnDisable()
    {
        controls.Player.ElementalSwitch.performed -= _ => SwitchMode();
        controls.Disable();
    }

    private void SwitchMode()
    {
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
            return;

        // 攻撃中は切替禁止
        if (isMeleeAttacking) return;

        // クールダウン中は切替禁止
        if (Time.time - lastSwitchTime < switchCooldown) return;

        // 切替開始
        StartCoroutine(DoSwitchMode());
    }

    private IEnumerator DoSwitchMode()
    {
        isSwitching = true;
        lastSwitchTime = Time.time;

        // 切替中は移動不可・攻撃不可
        rb.velocity = Vector2.zero;
        moveInput = Vector2.zero;

        // 見た目・ステータス反映
        ModeState = (ModeState == PlayerModeState.Light)
            ? PlayerModeState.Dark
            : PlayerModeState.Light;

        ApplyModeVisual();
        ApplyModeStats();

        // ここでSE（後で入れる）
        // SoundManager.Instance.PlaySE("ModeSwitch");

        // ラグ（硬直時間）
        yield return new WaitForSeconds(switchLag);

        isSwitching = false;
    }


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

    private void ApplyModeStats()
    {
        currentMoveSpeed = (ModeState == PlayerModeState.Light)
            ? lightMoveSpeed
            : darkMoveSpeed;
    }

    void Update()
    {
        // ゲームがプレイ状態でなければできない
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
            return;
        // モード切替中移動不可
        if (isSwitching) return;

        // 攻撃中は入力値を 0 にする
        moveInput = isMeleeAttacking ? Vector2.zero : controls.Player.Move.ReadValue<Vector2>();

        animator.SetFloat("MoveSpeed", moveInput.magnitude);
        animator.SetBool("IsDarkForm", ModeState == PlayerModeState.Dark);

        // 左右判定
        if (moveInput.x > 0.01f) lastMoveDir = Vector2.right;
        else if (moveInput.x < -0.01f) lastMoveDir = Vector2.left;

        IsFacingLeft = lastMoveDir.x < 0;
        animator.SetBool("IsFacingLeft", IsFacingLeft);
    }


    void FixedUpdate()
    {
        // 攻撃中は一切移動させない
        if (isMeleeAttacking) return;
        // モード切替中移動不可
        if (isSwitching) return;

        rb.MovePosition(rb.position + moveInput * currentMoveSpeed * Time.fixedDeltaTime);
    }


    public void BeginMeleeAttack()
    {
        isMeleeAttacking = true;
        rb.velocity = Vector2.zero;
        moveInput = Vector2.zero;
    }

    public void EndMeleeAttack()
    {
        isMeleeAttacking = false;
    }
    /// <summary>
    /// 攻撃できるかどうか
    /// </summary>
    /// <returns></returns>
    public bool CanAttack()
    {
        if (isSwitching) return false;
        if (isMeleeAttacking) return false;

        if (GameManager.Instance == null ||
            GameManager.Instance.State != GameState.Playing)
            return false;

        return true;
    }


    private void HandlePlayerDeath()
    {
        EndMeleeAttack();
        moveInput = Vector2.zero;
        rb.velocity = Vector2.zero;
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
