using UnityEngine;

/// <summary>
/// 敵が攻撃してダメージを与えた
/// </summary>
[RequireComponent(typeof(EnemyBase))]
public class EnemyDamage : MonoBehaviour
{
    [SerializeField] EnemyBase enemyBase;
    private int contactDamage = 1;

    void Awake()
    {
        enemyBase.OnBalanceApplied += ApplyBalance;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 攻撃対象がプレイヤーだけなら
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (collision.gameObject.TryGetComponent(out IDamageable target))
            {
                target.TakeDamage(contactDamage, AttackType.Melee,collision.gameObject.transform.position);
            }
        }
    }
    /// <summary>
    /// バランス適用
    /// </summary>
    /// <param name="stat"></param>
    private void ApplyBalance(EnemyStat stat)
    {
        contactDamage = stat.attack;
    }
}
