using UnityEngine;

[RequireComponent(typeof(EnemyBase))]
public class MiniBossRanged : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private EnemyBase enemyBase;
    [Header("----- Bullet Prefab -----")]
    [SerializeField] private Bullet bulletPrefab;

    [Header("----- Danmaku Settings -----")]
    [SerializeField] private int danmakuCount = 8;      // 8way
    [SerializeField] private float danmakuInterval = 1f;
    [SerializeField] private float danmakuBulletSpeed = 4f;
    [SerializeField] private float danmakuRotationSpeed = 15f;
    private int snipeBulletDamageRate = 2;

    [Header("----- Aim Shot Settings -----")]
    [SerializeField] private float aimInterval = 3f;
    [SerializeField] private float aimBulletSpeed = 10f;
    private int bulletDamage = 1;

    private float danmakuTimer = 0f;
    private float aimTimer = 0f;
    private float rotationOffset = 0f;

    private Transform player;

    void Awake()
    {
        enemyBase.OnBalanceApplied += ApplyBalance;
    }
    void Start()
    {
        player = PlayerManager.Instance.MainPlayer.transform;
    }

    void Update()
    {
        if (player == null) return;

        danmakuTimer += Time.deltaTime;
        aimTimer += Time.deltaTime;

        // 常時nWAY弾幕
        if (danmakuTimer >= danmakuInterval)
        {
            danmakuTimer = 0f;
            FireDanmaku();
        }

        // プレイヤー狙撃弾
        if (aimTimer >= aimInterval)
        {
            aimTimer = 0f;
            FireAimShot();
        }

        // 弾幕の毎回角度ずらし
        rotationOffset += danmakuRotationSpeed * Time.deltaTime;
    }

    // ------------------------------------------------------
    // NWAY 弾幕
    // ------------------------------------------------------
    void FireDanmaku()
    {
        float angleStep = 360f / danmakuCount;

        for (int i = 0; i < danmakuCount; i++)
        {
            float angle = rotationOffset + angleStep * i;
            Vector2 dir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            var b = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            b.Init(dir, BulletOwner.Enemy, bulletDamage, danmakuBulletSpeed,enemyBase.EnemyId);
        }
    }

    // ------------------------------------------------------
    // プレイヤー狙撃弾
    // ------------------------------------------------------
    void FireAimShot()
    {
        Vector2 dir = (player.position - transform.position).normalized;

        var b = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        b.Init(dir, BulletOwner.Enemy, snipeBulletDamageRate * bulletDamage, aimBulletSpeed,enemyBase.EnemyId);
    }

    /// <summary>
    /// バランス適用
    /// </summary>
    /// <param name="stat"></param>
    private void ApplyBalance(EnemyStat stat)
    {
        bulletDamage = stat.attack;
    }
}
