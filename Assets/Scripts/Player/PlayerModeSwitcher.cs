using UnityEngine;
using System.Collections;

public class PlayerModeSwitcher : MonoBehaviour
{
    [SerializeField] private PlayerController controller;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerVisuals visuals;

    [Header("Mode Change")]
    [SerializeField] private float switchCooldown = 1.0f;
    [SerializeField] private float switchLag = 0.3f;

    private PlayerControls controls;
    private float lastSwitchTime = -999f;
    private bool isSwitching = false;

    public bool IsSwitching => isSwitching;

    void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Enable();
        controls.Player.ElementalSwitch.performed += _ => TrySwitch();
    }

    void OnDisable()
    {
        controls.Player.ElementalSwitch.performed -= _ => TrySwitch();
        controls.Disable();
    }

    private void TrySwitch()
    {
        if (!controller.CanSwitchMode()) return;

        if (Time.time - lastSwitchTime < switchCooldown) return;
        StartCoroutine(DoSwitch());
    }

    private IEnumerator DoSwitch()
    {
        isSwitching = true;
        lastSwitchTime = Time.time;

        movement.MoveStop();

        // 状態切り替え
        controller.ToggleMode();

        // 見た目
        visuals.ApplyModeVisual(controller.ModeState);

        // ステータス
        controller.ApplyModeStatsFromOutside();

        // ラグ
        yield return new WaitForSeconds(switchLag);

        isSwitching = false;
    }
}