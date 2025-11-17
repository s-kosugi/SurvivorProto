using System.Collections;
using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PlayerController playerController;

    [Header("Attack Settings")]
    public float attackCooldown = 1.0f;
    public float attackRange = 2.0f;
    public float attackAngle = 120f;
    public int attackDamage = 2;
    public float knockbackForce = 3f;

    private bool isAttacking = false;
    private float lastAttackTime;

    [Header("References")]
    public Transform attackOrigin;        // 攻撃基準点
    public GameObject attackEffectPrefab; // 斬撃エフェクト等（任意）

    [Header("Attack Offset (Per Direction)")]
    [SerializeField] private float offsetRightX = 1.0f;
    [SerializeField] private float offsetLeftX = 1.0f;

    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Enable();
        controls.Player.Shoot.performed += _ => TryAttack();
    }

    void OnDisable()
    {
        controls.Player.Shoot.performed -= _ => TryAttack();
        controls.Disable();
    }

    void TryAttack()
    {
        if (playerController.ModeState == PlayerModeState.Light) return;
        if (Time.time - lastAttackTime < attackCooldown || isAttacking) return;

        StartCoroutine(DoAttack());
    }

    IEnumerator DoAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        // 若干の攻撃準備時間
        yield return new WaitForSeconds(0.15f);

         // ---- 方向別オフセット適用 ----
        float offset = playerController.IsFacingLeft ? offsetLeftX : offsetRightX;

        Vector2 attackPos = (Vector2)attackOrigin.position
                          + new Vector2(playerController.IsFacingLeft ? -offset : offset, 0f);


        // ================================
        // ② エフェクト生成（任意）
        // ================================
        if (attackEffectPrefab != null)
        {
            var fx = Instantiate(attackEffectPrefab, attackPos, Quaternion.identity);

            // 左向きならエフェクトを反転
            if (playerController.IsFacingLeft)
            {
                var scale = fx.transform.localScale;
                scale.x *= -1;
                fx.transform.localScale = scale;
            }
        }

        // ================================
        // ③ 範囲判定（OverlapSphere → ここを中心に）
        // ================================
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPos, attackRange);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            Vector2 hitDir = (hit.transform.position - (Vector3)attackPos).normalized;

            // ① 弱点持ち敵
            if (hit.TryGetComponent(out EnemyDarkWeak darkWeak))
            {
                darkWeak.ApplyWeaknessDamage(
                    attackDamage,
                    PlayerModeState.Dark,
                    AttackType.Melee,
                    hitDir
                );
                continue;
            }

            // ② 通常敵
            if (hit.TryGetComponent(out EnemyHealth enemy))
            {
                enemy.TakeDamage(attackDamage, AttackType.Melee, this.transform.position);

                if (hit.TryGetComponent(out Rigidbody2D rb))
                {
                    rb.AddForce(hitDir * knockbackForce, ForceMode2D.Impulse);
                }
                continue;
            }
        }

        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
    }

    // Scene可視化
    void OnDrawGizmosSelected()
    {
        if (attackOrigin == null) return;

        // 向き依存で可視化
        bool isLeft = false;
        if (playerController != null) isLeft = playerController.IsFacingLeft;

        Vector2 attackDirection = isLeft ? Vector2.left : Vector2.right;
        Vector2 attackPos = (Vector2)attackOrigin.position + (attackDirection * attackRange * 0.5f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos, attackRange);
    }
}
