using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerCore core;

    [Header("Shoot Settings")]
    [SerializeField] private float shootCooldown = 0.3f;
    [SerializeField] private float spreadAngle = 90f; 
    [SerializeField] private int spreadCount = 8;

    private float nextShootTime;

    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
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
    /// 光モードの攻撃（360°全方向ショット）
    /// </summary>
        void ShootLight()
    {
        int count = GetLightSpreadCount();
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = angleStep * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;

            var bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            bulletObj.GetComponent<Bullet>().SetDirection(dir);
        }
    }

    /// <summary>
    /// 闇モードのショット攻撃(※未実装)
    /// </summary>
    void ShootDark()
    {
        
    }

    int GetLightSpreadCount()
    {
        // 強化段階 → 弾数変換
        switch (core.attackStats.LightShotLevel)
        {
            case 0: return 4;   // 初期
            case 1: return 8;
            case 2: return 16;
            case 3: return 24;  // 好みで追加
            default: return 32; // 最終系
        }
    }

}
