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
    }

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

    /// <summary>
    /// 必要経験値、ゆる指数カーブ
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public int GetRequiredExp(int level)
    {
        // 固め指数カーブ（baseExp × 1.5^(level-1)）
        float curve = Mathf.Pow(1.5f, level - 1);
        int required = Mathf.RoundToInt(baseExp * curve);

        // 必要EXPが1未満になることを避ける（序盤の安定化）
        return Mathf.Max(required, baseExp);
    }
    /// <summary>
    /// デスペナルティ
    /// </summary>
    public void ApplyDeathPenalty()
    {
        // Light / Dark の経験値をレートをかけて減らす
        lightExp = Mathf.FloorToInt(lightExp * deathPenaltRate);
        darkExp  = Mathf.FloorToInt(darkExp * deathPenaltRate);

        // 必要ならUI更新イベントなどを呼ぶ
        OnExpChanged?.Invoke();
    }


    // -----------------------
    //   Light Level Up
    // -----------------------
    private void TryLevelUpLight()
    {
        int req = GetRequiredExp(lightLevel);

        while (lightExp >= req)
        {
            lightExp -= req;
            lightLevel++;

            Debug.Log($"[LEVEL UP] Light → {lightLevel}");
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
            OnDarkLevelUp();

            req = GetRequiredExp(darkLevel);
        }
    }

    // -----------------------
    //   Buff Logic
    // -----------------------
    private void OnLightLevelUp()
    {
        core.attackStats.LightPower += 1;
        playerShooting.ApplyLightGrowth(lightLevel);
        playerHealth.RecalculateMaxHP(lightLevel, darkLevel);
        SoundManager.Instance.PlaySE("LevelUpLight");
        Debug.Log($"[BUFF] 光攻撃強化 → LightPower = {core.attackStats.LightPower}");
    }

    private void OnDarkLevelUp()
    {
        // 基礎攻撃力アップ
        core.attackStats.DarkPower += 1;

        // HP成長
        playerHealth.RecalculateMaxHP(lightLevel, darkLevel);

        // コンボ段数・火力の成長を近接へ反映！
        if (meleeAttack != null)
        {
            meleeAttack.ApplyComboCount(darkLevel);
            meleeAttack.ApplyComboBonus(darkLevel);

            Debug.Log($"[BUFF] 闇攻撃強化（近接成長反映） → Lv{darkLevel}");
        }
        else
        {
            Debug.LogWarning("[WARN] meleeAttack がセットされていません。成長効果が反映されません！");
        }
        SoundManager.Instance.PlaySE("LevelUpDark");
        Debug.Log($"[BUFF] 闇攻撃強化 → DarkPower = {core.attackStats.DarkPower}");
    }
}
