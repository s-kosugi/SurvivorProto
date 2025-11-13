using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Shoot Settings")]
    [SerializeField] private float shootCooldown = 0.3f;
    [SerializeField] private float spreadAngle = 90f; 
    [SerializeField] private int spreadCount = 8;

    private float nextShootTime;

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
        if (Time.time < nextShootTime) return;
        nextShootTime = Time.time + shootCooldown;

        if (playerController.ModeState == PlayerModeState.Light)
        {
            ShootLight();
        }
        else
        {
            ShootDark();  // 後で追加する
        }
    }

    /// <summary>
    /// 光モードの攻撃（360°全方向ショット）
    /// </summary>
    void ShootLight()
    {
        float angleStep = 360f / spreadCount;

        for (int i = 0; i < spreadCount; i++)
        {
            float angle = spreadAngle + angleStep * i;
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
}
