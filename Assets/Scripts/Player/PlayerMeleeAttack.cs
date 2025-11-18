using System.Collections;
using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PlayerController playerController;

    [Header("Attack Settings")]
    public float attackCooldown = 0.5f;
    public float attackRange = 1.8f;
    public float attackAngle = 120f;
    public int attackDamage = 2;
    public float knockbackForce = 3f;

    [Header("Timing")]
    [SerializeField] private float attackRecoveryDuration = 0.20f; // 次の攻撃入力が可能になるまでの時間
    [SerializeField] private float moveLockDuration = 0.25f;       // 足を止めておく時間
    [SerializeField] private float stepForwardDistance = 0.25f;    // 踏み込み距離

    private bool isAttacking = false;
    private float lastAttackTime;

    [Header("References")]
    public Transform attackOrigin;
    public GameObject attackEffectPrefab;

    [Header("Attack Offset (Per Direction)")]
    [SerializeField] private float offsetRightX = 1.0f;
    [SerializeField] private float offsetLeftX = 1.0f;

    [Header("Debug Rendering")]
    [SerializeField] private bool debugShowHitbox = true;
    [SerializeField] private float debugDisplayTime = 0.15f;
    [SerializeField] private Material debugLineMaterial;
    [SerializeField] private Color debugColor = Color.yellow;
    [SerializeField] private float debugLineWidth = 0.05f;

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

        // 再攻撃制御
        if (Time.time - lastAttackTime < attackCooldown) return;
        if (isAttacking) return;

        StartCoroutine(DoAttack());
    }

    IEnumerator DoAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        // 近接移動制御ON
        playerController.BeginMeleeAttack();

        // 踏み込み
        Vector2 stepDir = playerController.IsFacingLeft ? Vector2.left : Vector2.right;
        transform.position += (Vector3)(stepDir * stepForwardDistance);

        // 即攻撃発生
        ExecuteAttack();

        // ▼ 再攻撃可能タイミング：攻撃判定が終わった瞬間
        yield return new WaitForSeconds(attackRecoveryDuration);
        isAttacking = false;

        // ▼ 移動ロックはもう少し長く続く可能性あり
        float extraLock = Mathf.Max(0f, moveLockDuration - attackRecoveryDuration);
        if (extraLock > 0f)
            yield return new WaitForSeconds(extraLock);

        // 近接移動制御解除
        playerController.EndMeleeAttack();
    }

    private void ExecuteAttack()
    {
        float offset = playerController.IsFacingLeft ? offsetLeftX : offsetRightX;

        Vector2 attackPos = (Vector2)attackOrigin.position +
            new Vector2(playerController.IsFacingLeft ? -offset : offset, 0f);

#if UNITY_EDITOR
        // 攻撃判定デバッグ表示
        if (debugShowHitbox)
            StartCoroutine(ShowDebugHitCircle(attackPos, attackRange));
#endif

        // エフェクト生成
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
    }

    void OnDrawGizmosSelected()
    {
        if (attackOrigin == null) return;
        bool isLeft = (playerController != null) && playerController.IsFacingLeft;
        Vector2 attackDirection = isLeft ? Vector2.left : Vector2.right;

        Vector2 attackPos = (Vector2)attackOrigin.position + (attackDirection * attackRange * 0.5f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos, attackRange);
    }

     // =======================================================================
    // ★ 攻撃判定が起きた瞬間～短時間のみ可視化するコルーチン
    // =======================================================================
    private IEnumerator ShowDebugHitCircle(Vector2 center, float radius, int segments = 30)
    {
        GameObject lineObj = new GameObject("DebugHitCircle");
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();

        lr.positionCount = segments + 1;
        lr.loop = true;
        lr.material = debugLineMaterial != null ? debugLineMaterial : new Material(Shader.Find("Sprites/Default"));
        lr.startColor = debugColor;
        lr.endColor = debugColor;
        lr.startWidth = debugLineWidth;
        lr.endWidth = debugLineWidth;
        lr.sortingOrder = 999; // 手前に表示

        for (int i = 0; i <= segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2f;
            Vector2 pos = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            lr.SetPosition(i, pos);
        }

        yield return new WaitForSeconds(debugDisplayTime);

        Destroy(lineObj);
    }

}
