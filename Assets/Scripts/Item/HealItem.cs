using UnityEngine;

public class HealItem : ItemBase
{
    [Header("Heal Settings")]
    public bool usePercentage = true;   // ← 割合回復スイッチ
    [Range(0f, 1f)]
    public float healPercent = 0.33f;   // ← 33%回復

    public int healAmount = 10;         // ← 固定値を使いたい時用（今後用）

    public override void Collect(PlayerCore player)
    {
        if (player != null && player.health != null)
        {
            if (usePercentage)
            {
                // MaxHP × 割合 → 小数点切り上げ
                int amount = Mathf.CeilToInt(player.health.MaxHP * healPercent);
                player.health.RecoverHP(amount);
            }
            else
            {
                // 固定値回復（従来通り）
                player.health.RecoverHP(healAmount);
            }
        }

        Destroy(gameObject);
    }
}
