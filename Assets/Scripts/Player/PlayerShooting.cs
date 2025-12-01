using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerCore core;
    [SerializeField] private PlayerMovement playerMovement;
    public PlayerGrowthConfig growthConfig;

    [Header("Current Light Stats")]
    private int frontShotCount = 1;
    private float frontAngle = 0f; // 扇形の角度

    private int nWayCount = 0;

    [Header("Shoot Settings")]
    [SerializeField] private float shootCooldown = 0.3f;

    private float nextShootTime;

    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
        ApplyLightGrowth(1);
    }

    void OnEnable()
    {
        controls.Enable();
        controls.Player.Shoot.performed += _ => TryShoot();
    }

    void OnDisable()
    {
        controls.Player.Shoot.performed -= _ => TryShoot();
        controls.Disable();
    }

    /// <summary>
    /// 入力時に撃つかどうかを判定し、モードに応じて攻撃を切り替える
    /// </summary>
    void TryShoot()
    {
        // まず PlayerController 側に問い合わせ
        if (!playerController.CanAttack())
            return;

        if (Time.time < nextShootTime) return;
        nextShootTime = Time.time + shootCooldown;

        if (playerController.ModeState == PlayerModeState.Light)
        {
            ShootLight();
        }
        else
        {
            ShootDark();
        }
    }

    /// <summary>
    /// 光モードの攻撃（nWay弾＋前方扇ショット)
    /// </summary>
    void ShootLight()
    {
        // --- 正面ショット（扇形） ---
        if (frontShotCount > 0)
            ShootFrontShots(frontShotCount, frontAngle);

        // --- 全方向ショット（NWay） ---
        if (nWayCount > 1)
            ShootNWay(nWayCount);
    }
    /// <summary>
    /// 前方ショット
    /// </summary>
    void ShootFrontShots(int count, float angle)
    {
        if (count <= 0) return;

        // プレイヤーの向き取得
        bool isFacingLeft = playerMovement != null && playerMovement.IsFacingLeft;

        if (count == 1)
        {
            if( isFacingLeft )
                SpawnBullet(Vector2.left);
            else
                SpawnBullet(Vector2.right);
            return;
        }
        // 扇形の中心角（右向きは0°, 左向きは180°）
        float baseAngle = isFacingLeft ? 180f : 0f;

        float half = angle * 0.5f;
        float step = angle / (count - 1);

        for (int i = 0; i < count; i++)
        {
            float offset = -half + step * i;
            float totalAngle = baseAngle + offset;
            float rad = totalAngle * Mathf.Deg2Rad;

            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
            SpawnBullet(dir);
        }
    }
    /// <summary>
    /// Nway弾(360度)
    /// </summary>
    /// <param name="count"></param>
    void ShootNWay(int count)
    {
        float step = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = step * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
            SpawnBullet(dir);
        }
    }

    /// <summary>
    /// 闇モードのショット攻撃(※未実装)
    /// </summary>
    void ShootDark()
    {
        
    }
    /// <summary>
    /// 成長値の適用
    /// </summary>
    /// <param name="lightLevel"></param>
    public void ApplyLightGrowth(int lightLevel)
    {
        // ---- 正面ショット成長 ----
        foreach (var g in growthConfig.lightFrontShotGrowths)
        {
            if (lightLevel >= g.level)
            {
                frontShotCount = g.frontShotCount;
                frontAngle = g.frontAngle;
            }
        }

        // ---- 全方向 N-Way 成長 ----
        nWayCount = 0;
        foreach (var g in growthConfig.lightNWayGrowths)
        {
            if (lightLevel >= g.level)
            {
                nWayCount = g.nWayCount;
            }
        }

        Debug.Log($"[LightGrowth] Lv{lightLevel}: Front={frontShotCount}, Angle={frontAngle}, NWay={nWayCount}");
    }

    /// <summary>
    /// 弾生成
    /// </summary>
    /// <param name="dir"></param>
    void SpawnBullet(Vector2 dir)
    {
        var obj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        obj.GetComponent<Bullet>().SetDirection(dir);
        // 弾発射音
        SoundManager.Instance.PlaySE("SmallShot");
    }

}
