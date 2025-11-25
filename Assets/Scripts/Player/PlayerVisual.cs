using UnityEngine;
using System.Collections.Generic;

public class PlayerVisuals : MonoBehaviour
{
    [Header("Aura")]
    [SerializeField] private GameObject lightAura;
    [SerializeField] private GameObject darkAura;

    [Header("Switch Effects")]
    [SerializeField] private GameObject lightFlashPrefab;
    [SerializeField] private GameObject darkFlashPrefab;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    private List<GameObject> spawnedEffects = new List<GameObject>();

    // ===== モードの見た目更新 =====
    public void ApplyModeVisual(PlayerModeState mode)
    {
        lightAura.SetActive(mode == PlayerModeState.Light);
        darkAura.SetActive(mode == PlayerModeState.Dark);

        var fxPrefab = (mode == PlayerModeState.Light)
            ? lightFlashPrefab
            : darkFlashPrefab;

        var fx = Instantiate(fxPrefab, transform.position, Quaternion.identity);
        spawnedEffects.Add(fx);

        animator.SetBool("IsDarkForm", mode == PlayerModeState.Dark);
    }

    // ===== 移動アニメ更新（Movementから呼ぶ） =====
    public void UpdateMoveAnimation(float moveSpeed)
    {
        animator.SetFloat("MoveSpeed", moveSpeed);
    }

    // ===== 向き更新（Movementから呼ぶ） =====
    public void UpdateFacing(bool isLeft)
    {
        animator.SetBool("IsFacingLeft", isLeft);
    }

    // ===== 死亡・リセット =====
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
