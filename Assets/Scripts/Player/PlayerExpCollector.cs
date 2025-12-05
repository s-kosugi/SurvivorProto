using UnityEngine;

public class PlayerExpCollector : MonoBehaviour
{
    [SerializeField] private PlayerCore core;
    [SerializeField] private PlayerController controller;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerMeleeAttack meleeAttack;
    [SerializeField] private PlayerShooting playerShooting;
    public event System.Action OnExpChanged;

    [Header("Current EXP")]
    public int lightExp = 0;
    public int darkExp = 0;

    [Header("Current Form")]
    public bool isLightForm = true;

    [Header("Current Level")]
    public int lightLevel = 1;
    public int darkLevel = 1;

    [Header("Level Settings")]
    [SerializeField] private int baseExp = 10;
    [SerializeField] private float deathPenaltRate = 0.7f;

    private void Awake()
    {
        // ----------- セーブデータ読み込み -----------
        lightLevel = SessionData.LightLevel <= 0 ? 1 : SessionData.LightLevel;
        darkLevel  = SessionData.DarkLevel  <= 0 ? 1 : SessionData.DarkLevel;

        lightExp = Mathf.Max(SessionData.LightExp, 0);
        darkExp  = Mathf.Max(SessionData.DarkExp, 0);

    }
    private void Start()
    {
        // ----------- 現在のレベルに応じた成長反映（SEなし） -----------
        ApplyLightGrowthRaw();
        ApplyDarkGrowthRaw();
    }

    // ===========================
    //       EXP ADD
    // ===========================
    public void AddExp(int amount)
    {
        if (controller.ModeState == PlayerModeState.Light)
        {
            lightExp += amount;
            Debug.Log($"[EXP] Light +{amount} → {lightExp}");
            TryLevelUpLight();
        }
        else
        {
            darkExp += amount;
            Debug.Log($"[EXP] Dark +{amount} → {darkExp}");
            TryLevelUpDark();
        }
        OnExpChanged?.Invoke();  // UIへ通知
    }

    // ===========================
    //     必要経験値
    // ===========================
    public int GetRequiredExp(int level)
    {
        float curve = Mathf.Pow(1.5f, level - 1);
        int required = Mathf.RoundToInt(baseExp * curve);

        return Mathf.Max(required, baseExp);
    }

    // ===========================
    //   デスペナルティ
    // ===========================
    public void ApplyDeathPenalty()
    {
        lightExp = Mathf.FloorToInt(lightExp * deathPenaltRate);
        darkExp  = Mathf.FloorToInt(darkExp * deathPenaltRate);

        OnExpChanged?.Invoke();
    }

    // ===========================
    //   Light Level Up
    // ===========================
    private void TryLevelUpLight()
    {
        int req = GetRequiredExp(lightLevel);

        while (lightExp >= req)
        {
            lightExp -= req;
            lightLevel++;

            Debug.Log($"[LEVEL UP] Light → {lightLevel}");

            // 強化適用（演出あり）
            OnLightLevelUp();

            req = GetRequiredExp(lightLevel);
        }
    }

    private void TryLevelUpDark()
    {
        int req = GetRequiredExp(darkLevel);

        while (darkExp >= req)
        {
            darkExp -= req;
            darkLevel++;

            Debug.Log($"[LEVEL UP] Dark → {darkLevel}");

            // 強化適用（演出あり）
            OnDarkLevelUp();

            req = GetRequiredExp(darkLevel);
        }
    }

    // ===========================
    //      強化ロジック
    // ===========================

    // --- SEなし・復元用 ---
    private void ApplyLightGrowthRaw()
    {
        // 本来の強化式に合わせて調整（+=1? level分?）
        core.attackStats.LightPower = lightLevel;
        playerShooting.ApplyLightGrowth(lightLevel);
        playerHealth.RecalculateMaxHP(lightLevel, darkLevel);
    }

    private void ApplyDarkGrowthRaw()
    {
        core.attackStats.DarkPower = darkLevel;
        playerHealth.RecalculateMaxHP(lightLevel, darkLevel);

        if (meleeAttack != null)
        {
            meleeAttack.ApplyComboCount(darkLevel);
            meleeAttack.ApplyComboBonus(darkLevel);
        }
    }

    // --- レベルアップ時（演出あり） ---
    private void OnLightLevelUp()
    {
        ApplyLightGrowthRaw();
        SoundManager.Instance.PlaySE("LevelUpLight");
        EffectLibrary.Instance.SpawnEffect(EffectType.LevelUpLight,core.transform.position, Quaternion.identity, null, 1);
        Debug.Log($"[BUFF] 光攻撃強化 → LightPower = {core.attackStats.LightPower}");
    }

    private void OnDarkLevelUp()
    {
        ApplyDarkGrowthRaw();
        SoundManager.Instance.PlaySE("LevelUpDark");
        EffectLibrary.Instance.SpawnEffect(EffectType.LevelUpDark,core.transform.position, Quaternion.identity, null, 1);
        Debug.Log($"[BUFF] 闇攻撃強化 → DarkPower = {core.attackStats.DarkPower}");
    }
}
