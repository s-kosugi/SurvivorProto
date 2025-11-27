using UnityEngine;

[RequireComponent(typeof(EnemyBase))]
public class EnemyDarkWeak : MonoBehaviour
{
    [Header("----- References -----")]
    [SerializeField] private EnemyBase enemyBase;

    [Header("----- Shooting (Ranged Attack) -----")]
    [SerializeField] private GameObject bulletPrefab;       // 敵の通常弾
    [SerializeField] private float shootInterval = 1.5f;    // 通常射撃間隔
    [SerializeField] private float stopShootRange = 1.2f;   // この距離未満で射撃停止
    [SerializeField] private float bulletSpeed = 8f;

    private int shootDamage = 1;
    private float shootTimer = 0f;

    [Header("----- Counter Attack -----")]
    [SerializeField] private GameObject counterBulletPrefab; // カウンター弾
    [SerializeField] private float counterSpeed = 10f;
    private int counterDamage = 1;

    [Header("----- Weakness Settings -----")]
    public float darkMeleeMultiplier = 1.8f;   // 闇近接 → 露骨に弱点
    public float lightBulletMultiplier = 0.7f; // 光の遠距離は硬い（弱点ではない）

    [Header("----- Knockback -----")]
    [SerializeField] private float knockbackPower = 0.2f;

    private Transform player;

    void Awake()
    {
        enemyBase.OnBalanceApplied += ApplyBalance;
    }
    private void Start()
    {
        player = PlayerManager.Instance.MainPlayer.transform;
    }

    private void Update()
    {
        if (!player) return;

        float dist = Vector2.Distance(transform.position, player.position);

        // --- 射撃（プレイヤーが遠距離の場合） ---
        if (dist > stopShootRange)
        {
            shootTimer += Time.deltaTime;
            if (shootTimer >= shootInterval)
            {
                ShootToPlayer();
                shootTimer = 0f;
            }
        }
    }

    // ======================================================
    //  射撃（通常攻撃）
    // ======================================================
    private void ShootToPlayer()
    {
        if (!bulletPrefab) return;

        Vector2 dir = (player.position - transform.position).normalized;

        var b = Instantiate(bulletPrefab, transform.position, Quaternion.identity)
                .GetComponent<Bullet>();

        b.Init(dir, BulletOwner.Enemy, shootDamage,bulletSpeed);
    }

    // ======================================================
    //  カウンター（光遠距離が当たったとき呼び出される）
    // ======================================================
    public void OnHitByBullet(Vector2 _)
    {
        if (!player) return;

        Vector2 dir = (player.position - transform.position).normalized;

        var cb = Instantiate(counterBulletPrefab, transform.position, Quaternion.identity)
                    .GetComponent<Bullet>();

        cb.Init(dir, BulletOwner.Enemy, counterDamage,counterSpeed);
    }


    // ======================================================
    //  ダメージ処理（攻撃側から呼ぶ）
    // ======================================================
    public void ApplyWeaknessDamage(int dmg, PlayerModeState form, AttackType type, Vector2 hitDir)
    {
        float final = dmg;

        // --- 闇近接：弱点（超DPS） ---
        if (form == PlayerModeState.Dark && type == AttackType.Melee)
        {
            final *= darkMeleeMultiplier;
            Knockback(hitDir);
        }
        // --- 光遠距離：硬い ---
        else if (form == PlayerModeState.Light && type == AttackType.Bullet)
        {
            final *= lightBulletMultiplier;
        }

        enemyBase.TakeDamage(Mathf.RoundToInt(final), type, transform.position);
    }

    private void Knockback(Vector2 dir)
    {
        transform.position += (Vector3)(dir * knockbackPower);
    }

    /// <summary>
    /// バランス適用
    /// </summary>
    /// <param name="stat"></param>
    private void ApplyBalance(EnemyStat stat)
    {
        shootDamage = stat.attack;
        counterDamage = stat.attack;
    }
}
