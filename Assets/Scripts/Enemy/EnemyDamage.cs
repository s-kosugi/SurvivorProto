using UnityEngine;

/// <summary>
/// 敵が攻撃してダメージを与えた
/// </summary>
public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private int contactDamage = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 攻撃対象がプレイヤーだけなら
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (collision.gameObject.TryGetComponent(out IDamageable target))
            {
                target.TakeDamage(contactDamage, AttackType.Melee);
            }
        }
    }
}
