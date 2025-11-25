using UnityEngine;

public class PlayerExpCollector : MonoBehaviour
{
    [SerializeField] private PlayerCore core;
    [SerializeField] private PlayerController controller;

    [Header("Current EXP")]
    public int lightExp = 0;
    public int darkExp = 0;

    [Header("Current Form")]
    public bool isLightForm = true;  

    [Header("Current Level")]
    public int lightLevel = 1;
    public int darkLevel = 1;

    [Header("Level Settings")]
    public int baseExp = 10;

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
    }

    private int GetRequiredExp(int level)
    {
        return level * baseExp;
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
        core.attackStats.LightShotLevel++;
        Debug.Log($"[BUFF] 光攻撃強化 → LightPower = {core.attackStats.LightPower}");
    }

    private void OnDarkLevelUp()
    {
        core.attackStats.DarkPower += 1;
        core.attackStats.DarkComboLevel++;
        Debug.Log($"[BUFF] 闇攻撃強化 → DarkPower = {core.attackStats.DarkPower}");
    }
}
