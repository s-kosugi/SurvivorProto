using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public PlayerModeState ModeState { get; private set; } = PlayerModeState.Light;
    public PlayerHealth Health => health;
    [SerializeField] private PlayerHealth health;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator animator;

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

    void Awake()
    {
        controls = new PlayerControls();
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
        ModeState = (ModeState == PlayerModeState.Light)
            ? PlayerModeState.Dark
            : PlayerModeState.Light;

        ApplyModeVisual();
        ApplyModeStats();
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
        // 入力取得
        moveInput = controls.Player.Move.ReadValue<Vector2>();

        // ---- Animatorへ値送信 ----
        animator.SetFloat("MoveSpeed", moveInput.magnitude);
        animator.SetBool("IsDarkForm", ModeState == PlayerModeState.Dark);

        // 左右判定
        if (moveInput.x > 0.01f) lastMoveDir = Vector2.right;
        else if (moveInput.x < -0.01f) lastMoveDir = Vector2.left;

        animator.SetBool("IsFacingLeft", lastMoveDir.x < 0);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * currentMoveSpeed * Time.fixedDeltaTime);
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
