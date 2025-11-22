using UnityEngine;

public class PlayerExpCollector : MonoBehaviour
{
    [Header("Current EXP")]
    public int lightExp = 0;
    public int darkExp = 0;

    [Header("Current Form")]
    public bool isLightForm = true;  
    // TODO: 後で PlayerFormManager と連動に変更する

    [Header("Level Settings")]
    public int lightLevel = 1;
    public int darkLevel = 1;

    // 次レベルまで必要なExp（仮）
    public int expPerLevel = 10;

    public void AddExp(int amount)
    {
        if (isLightForm)
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


    private void TryLevelUpLight()
    {
        while (lightExp >= expPerLevel)
        {
            lightExp -= expPerLevel;
            lightLevel++;

            Debug.Log($"[LEVEL UP] Light → {lightLevel}");

            // TODO: 光攻撃強化処理をここに
        }
    }

    private void TryLevelUpDark()
    {
        while (darkExp >= expPerLevel)
        {
            darkExp -= expPerLevel;
            darkLevel++;

            Debug.Log($"[LEVEL UP] Dark → {darkLevel}");

            // TODO: 闇攻撃強化処理をここに
        }
    }
}