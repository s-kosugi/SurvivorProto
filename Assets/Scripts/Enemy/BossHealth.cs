using UnityEngine;

public class BossHealth : EnemyBase
{
    public event System.Action<BossHealth> OnBossDead;
    public bool IsDead { get; private set; } = false;

    protected override void Start()
    {
        base.Start();

        EnemyManager.Instance?.RegisterEnemy(this.gameObject);
    }

    protected override void Die()
    {
        if (IsDead) return;
        IsDead = true;

        // スコア加算（必要なら調整可）
        GameManager.Instance?.AddScore(score);

        // EnemyManager から解除
        EnemyManager.Instance?.UnregisterEnemy(this.gameObject);

        // WaveEventManager に通知
        OnBossDead?.Invoke(this);

        Destroy(gameObject);
    }
}
