using System.Collections;
using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerCore core;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerGrowthConfig growthConfig;

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
    private bool bufferedInput = false;

    [Header("Combo Settings")]
    [SerializeField] private float comboInputWindow = 0.20f;

    private int currentMaxCombo = 1;  // 初期状態
    private int bonusFirst = 0;
    private int bonusSecond = 0;
    private int bonusThird = 0;

    private int currentComboStep = 0;
    private float comboTimer = 0f;

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
        controls.Player.Shoot.performed += _ => OnAttackInput();
    }

    void OnDisable()
    {
        controls.Player.Shoot.performed -= _ => OnAttackInput();
        controls.Disable();
    }
    void Update()
    {
        // コンボ入力受付中タイマー更新
        if (currentComboStep > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
            {
                ResetCombo();
            }
        }
    }
    private void OnAttackInput()
    {
        // 攻撃可能かどうか
        if (!playerController.CanAttack()) return;

        if (playerController.ModeState == PlayerModeState.Light) return;

        // 攻撃中なら予約
        if (isAttacking)
        {
            if (!playerController.CanAttack()) return;  // 切替中なら予約もしない
            bufferedInput = true;
            return;
        }

        // コンボ開始 or 継続受付中
        if (currentComboStep == 0 || comboTimer > 0f)
        {
            currentComboStep++;
            StartCoroutine(DoAttack());
        }
    }
    private IEnumerator DoAttack()
    {
        isAttacking = true;
        comboTimer = comboInputWindow;

        playerController.BeginMeleeAttack();

        Vector2 stepDir = playerMovement.IsFacingLeft ? Vector2.left : Vector2.right;
        transform.position += (Vector3)(stepDir * GetStepForward());

        ExecuteAttack();

        yield return new WaitForSeconds(attackRecoveryDuration);
        // コンボ段階に応じて硬直延長
        float lockTime = GetMoveLockDuration();
        float extraLock = Mathf.Max(0f, lockTime - attackRecoveryDuration);
        if (extraLock > 0f)
            yield return new WaitForSeconds(extraLock);

        isAttacking = false;
        playerController.EndMeleeAttack();

        // 次段が存在するなら入力待ち
        if (currentComboStep < currentMaxCombo  && bufferedInput)
        {
            bufferedInput = false;
            currentComboStep++;
            StartCoroutine(DoAttack());
        }
        else if (currentComboStep >= currentMaxCombo )
        {
            // 3段目後はフィニッシュ
            ResetCombo();
        }
    }
    private void ExecuteAttack()
    {
        float offset = playerMovement.IsFacingLeft ? offsetLeftX : offsetRightX;

        Vector2 attackPos = (Vector2)attackOrigin.position +
            new Vector2(playerMovement.IsFacingLeft ? -offset : offset, 0f);

        // エフェクト生成
        if (attackEffectPrefab != null)
        {
            SoundManager.Instance.PlaySE("MeleeAttack");
            var fx = Instantiate(attackEffectPrefab, attackPos, Quaternion.identity);

            // 段階ごとに拡大
            float scaleRate = GetEffectScale(currentComboStep);
            fx.transform.localScale *= scaleRate;

            // 左向きならエフェクトを反転
            if (playerMovement.IsFacingLeft)
            {
                var scale = fx.transform.localScale;
                scale.x *= -1;
                fx.transform.localScale = scale;
            }
}


        Debug.Log("currentComboStep = " + currentComboStep);
        // コンボ段階で攻撃範囲を決定
        float radius = GetAttackRadius(currentComboStep);

#if UNITY_EDITOR
        // 攻撃判定デバッグ表示
        if (debugShowHitbox)
            StartCoroutine(ShowDebugHitCircle(attackPos, radius));
#endif

        // ================================
        // ③ 範囲判定（OverlapSphere → ここを中心に）
        // ================================
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPos, radius);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            Vector2 hitDir = (hit.transform.position - (Vector3)attackPos).normalized;

            // 弱点持ち敵
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

            // 通常敵
            if (hit.TryGetComponent(out EnemyBase enemy))
            {
                int bonus = GetComboBonus(currentComboStep);
                enemy.TakeDamage(attackDamage + bonus, AttackType.Melee, this.transform.position);

                if (hit.TryGetComponent(out Rigidbody2D rb))
                {
                    rb.AddForce(hitDir * knockbackForce, ForceMode2D.Impulse);
                }
                continue;
            }
        }
    }

    // レベルからコンボボーナスを適用
    public void ApplyComboBonus(int darkLevel)
    {
        bonusFirst = 0;
        bonusSecond = 0;
        bonusThird = 0;

        foreach (var g in growthConfig.darkComboGrowths)
        {
            if (darkLevel >= g.level)
            {
                bonusFirst += g.firstBonus;
                bonusSecond += g.secondBonus;
                bonusThird += g.thirdBonus;
            }
        }
    }
    // レベルから最大コンボ数を適用
    public void ApplyComboCount(int darkLevel)
    {
        int result = 1;

        foreach (var g in growthConfig.darkComboCountGrowths)
        {
            if (darkLevel >= g.level)
                result = g.maxCombo;
        }

        currentMaxCombo = result;
    }
    /// <summary>
    /// コンボダメージボーナス取得
    /// </summary>
    private int GetComboBonus(int step)
    {
        step = Mathf.Min(step, 3); // 4段目以降は3段目扱い
        switch (step)
        {
            case 1: return bonusFirst;
            case 2: return bonusSecond;
            case 3: return bonusThird;
        }
        return 0;
    }
    private float GetAttackRadius(int step)
    {
        step = Mathf.Min(step, 3); // 4段目以降は3段目扱い
        switch (step)
        {
            case 1: return attackRange;
            case 2: return attackRange * 1.15f;
            case 3: return attackRange * 1.35f;
            default: return attackRange;
        }
    }
    private float GetStepForward()
    {
        int step = Mathf.Min(currentComboStep, 3); // 3段目以降は3段目と同じ
        switch (step)
        {
            case 1: return stepForwardDistance;
            case 2: return stepForwardDistance * 1.3f;
            case 3: return stepForwardDistance * 1.6f;
            default: return stepForwardDistance;
        }
    }
    private float GetMoveLockDuration()
    {
        int step = Mathf.Min(currentComboStep, 3);
        switch (step)
        {
            case 1: return moveLockDuration * 1.0f;
            case 2: return moveLockDuration * 1.5f;
            case 3: return moveLockDuration * 1.9f;
            default: return moveLockDuration;
        }
    }
    private float GetEffectScale(int step)
    {
        step = Mathf.Min(currentComboStep, 3);
        switch (step)
        {
            case 1: return 1.0f;
            case 2: return 1.25f;
            case 3: return 1.5f;
            default: return 1.0f;
        }
    }
    private void ResetCombo()
    {
        currentComboStep = 0;
        comboTimer = 0f;
    }


    void OnDrawGizmosSelected()
    {
        if (attackOrigin == null) return;
        bool isLeft = (playerMovement != null) && playerMovement.IsFacingLeft;
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
