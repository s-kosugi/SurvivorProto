using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Type")]
    [SerializeField] private EnemyID enemyId; 
    public EnemyID EnemyId => enemyId;
    [SerializeField]EnemyDropper dropper;
    [SerializeField]EnemyHitEffect enemyHitEffect;

    int maxHP = 3;
    private int currentHP;
    protected int score = 10;
    // バランス適用イベント
    public event System.Action<EnemyStat> OnBalanceApplied;

    protected virtual void Start()
    {
        var stat = GameBalanceLoader.Current.GetStat(enemyId);
        ApplyBalance(stat);
        OnBalanceApplied?.Invoke(stat);

        currentHP = maxHP;
    }
    public virtual void ApplyBalance(EnemyStat stat)
    {
        maxHP = stat.maxHP;
        score = stat.score;
    }

    public void TakeDamage(int damage,AttackType attackType,Vector3 attackerPos)
    {
        currentHP -= damage;

        // 攻撃タイプに応じたエフェクト生成
        enemyHitEffect.PlayHitEffect(attackType,attackerPos);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        // アイテムドロップ処理
        if (dropper != null)
            dropper.ExecuteDrop(transform.position);

        GameManager.Instance?.AddScore(score);
        EnemyManager.Instance.UnregisterEnemy(this.gameObject);
        Destroy(gameObject);
    }

}