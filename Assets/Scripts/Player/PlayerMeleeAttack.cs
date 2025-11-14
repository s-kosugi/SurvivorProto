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
            if (!hit.CompareTag("Enemy")) continue;

            Vector2 hitDir = (hit.transform.position - attackOrigin.position).normalized;

            // ① 近接弱点（EnemyDarkWeak）
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

            // ② 通常敵（EnemyHealth）
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackOrigin.position, attackRange);
    }
}
