using UnityEngine;

public class ExpItem : ItemBase
{
    [Header("Exp Value")]
    public int expValue = 1;

    private Rigidbody2D rb;

    protected override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody2D>();

        // ちょっと散らばる演出（任意）
        if (rb != null)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            rb.AddForce(dir * 1.2f, ForceMode2D.Impulse);
        }
    }

    public override void Collect(PlayerCore player)
    {
        if (player != null && player.expCollector != null)
        {
            player.expCollector.AddExp(expValue);
        }

        // アイテム消去
        Destroy(gameObject);
    }
}
