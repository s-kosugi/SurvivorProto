using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem; // 新InputSystem用

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public PlayerModeState ModeState { get; private set; } = PlayerModeState.Light;
    public PlayerHealth Health => health;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite lightSprite;
    [SerializeField] private Sprite darkSprite;

    [SerializeField] private PlayerHealth health;

    [Header("Mode Effects")]
    [SerializeField] private GameObject lightAura;
    [SerializeField] private GameObject darkAura;
    [SerializeField] private GameObject switchEffectPrefab;
    [SerializeField] private GameObject lightFlashPrefab;
    [SerializeField] private GameObject darkFlashPrefab;

    [Header("ステータス")]
    public float lightMoveSpeed = 5f;
    public float darkMoveSpeed = 7f;


    private Rigidbody2D rb;
    private PlayerControls controls;
    private Vector2 moveInput;
    public float moveSpeed = 5f;

    private float currentMoveSpeed = 5f;
    private List<GameObject> spawnedEffects = new List<GameObject>();
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controls = new PlayerControls();
    }

    private void Start()
    {
        ApplyModeStats();  // 初期モードの移動速度をセット
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
    /// <summary>
    /// 光闇モードの見た目切り替え適用
    /// </summary>
    private void ApplyModeVisual()
    {
        spriteRenderer.sprite = (ModeState == PlayerModeState.Light)
            ? lightSprite
            : darkSprite;

        // 光/闇のオーラ切り替え
        lightAura.SetActive(ModeState == PlayerModeState.Light);
        darkAura.SetActive(ModeState == PlayerModeState.Dark);

        // 切り替えエフェクト
        if (ModeState == PlayerModeState.Light)
            switchEffectPrefab = lightFlashPrefab;
        else
            switchEffectPrefab = darkFlashPrefab;
        var fx = Instantiate(switchEffectPrefab, transform.position, Quaternion.identity);
        spawnedEffects.Add(fx);
    }

    private void ApplyModeStats()
    {
        // 仮で移動速度を変える例
        currentMoveSpeed = (ModeState == PlayerModeState.Light)
            ? lightMoveSpeed
            : darkMoveSpeed;
    }

    void FixedUpdate()
    {
        Vector2 input = controls.Player.Move.ReadValue<Vector2>();
        rb.MovePosition(rb.position + input * currentMoveSpeed * Time.fixedDeltaTime);
    }

    // リセット用：全部消す
    public void ClearAllEffects()
    {
        // 常時エフェクトOFF
        lightAura.SetActive(false);
        darkAura.SetActive(false);

        // 切替瞬間エフェクト削除
        foreach (var fx in spawnedEffects)
        {
            if (fx != null) Destroy(fx);
        }
        spawnedEffects.Clear();
    }
}
