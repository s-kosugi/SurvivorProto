using System.Collections;
using UnityEngine;

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
    public Transform attackOrigin;

    private PlayerControls controls;
    private PlayerController playerController;

    void Awake()
    {
        controls = new PlayerControls();
        playerController = GetComponent<PlayerController>();
    }

    void OnEnable()
    {
        controls.Enable();
        controls.Player.Shoot.performed += _ => TryAttack(); 
        // → 光と同じ「Shoot」入力で差し替え（後述）
    }

    void OnDisable()
    {
        controls.Player.Shoot.performed -= _ => TryAttack();
        controls.Disable();
    }

    void TryAttack()
    {
        // 光モードなら何もしない
        if (playerController.ModeState == PlayerModeState.Light) return;

        if (Time.time - lastAttackTime < attackCooldown || isAttacking) return;

        StartCoroutine(DoAttack());
    }

    IEnumerator DoAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        yield return new WaitForSeconds(0.2f);

        // 攻撃エフェクト
        EffectLibrary.Instance.SpawnEffect(
            EffectType.Slash,
            attackOrigin.position,
            default,
            spriteRenderer
        );

        // 範囲ダメージ
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackOrigin.position, attackRange);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                var enemy = hit.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage(attackDamage, AttackType.Melee, this.transform.position);

                    // ノックバック
                    Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        Vector2 dir = (hit.transform.position - attackOrigin.position).normalized;
                        rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
                    }
                }
            }
        }

        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
    }

    // Scene可視化
    void OnDrawGizmosSelected()
    {
        if (attackOrigin == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackOrigin.position, attackRange);
    }
}
