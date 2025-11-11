using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMeleeAttack : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [Header("Attack Settings")]
    public float attackCooldown = 1.0f;
    public float attackRange = 2.0f;
    public float attackAngle = 120f;
    public int attackDamage = 2;
    public float knockbackForce = 3f;

    private bool isAttacking = false;
    private float lastAttackTime;

    [Header("References")]
    public Transform attackOrigin; // 攻撃基準点（プレイヤー前方）

    void Update()
    {
        if (Keyboard.current.vKey.wasPressedThisFrame) // VKeyで攻撃
        {
            TryAttack();
        }
    }

    void TryAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown || isAttacking) return;
        StartCoroutine(DoAttack());
    }

    IEnumerator DoAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        yield return new WaitForSeconds(0.2f); // 攻撃タイミング

        // 攻撃エフェクト生成
        EffectLibrary.Instance.SpawnEffect(EffectType.Slash, attackOrigin.position, default,spriteRenderer);

        // ★2D用：Overlapping Circleで検出
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackOrigin.position, attackRange);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                var enemy = hit.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage(attackDamage, AttackType.Melee,this.transform.position);

                    // ノックバック（2D版）
                    Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        Vector2 dir = (hit.transform.position - attackOrigin.position).normalized;
                        rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
                    }
                }
                else
                {
                    Debug.LogWarning($"Enemy {hit.name} has no EnemyHealth!");
                }
            }
        }

        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
    }

    // 攻撃範囲をSceneビューで可視化
    void OnDrawGizmosSelected()
    {
        if (attackOrigin == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackOrigin.position, attackRange);
    }
}
