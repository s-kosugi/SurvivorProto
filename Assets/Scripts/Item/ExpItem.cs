using UnityEngine;

public class ExpItem : ItemBase
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject pickupEffectPrefab;

    [Header("Exp Value")]
    [SerializeField] private int expValue = 1;

    protected override void Start()
    {
        base.Start();

        // 軽い散らばり演出（任意）
        if (rb != null)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            rb.AddForce(dir * 1.2f, ForceMode2D.Impulse);
        }
    }

    public override void Collect(PlayerCore player)
    {
        // 経験値付与
        if (player != null && player.expCollector != null)
        {
            player.expCollector.AddExp(expValue);
        }

        // 取得エフェクト生成（Prefab が設定されている場合のみ）
        if (pickupEffectPrefab != null)
        {
            Instantiate(
                pickupEffectPrefab,
                transform.position,
                Quaternion.identity
            );
        }
        else
        {
            Debug.LogWarning($"{name}: pickupEffectPrefab が設定されていません！");
        }

        // アイテム消去
        Destroy(gameObject);
    }
}
