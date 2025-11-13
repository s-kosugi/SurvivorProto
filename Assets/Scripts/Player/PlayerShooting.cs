using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float shootCooldown = 0.3f;
    [SerializeField] private float spreadAngle = 90f;
    [SerializeField] private int spreadCount = 8;

    private float nextShootTime;

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && Time.time >= nextShootTime)
        {
            Shoot();
            nextShootTime = Time.time + shootCooldown;
        }
    }

/// <summary>
/// 弾発射ロジック
/// </summary>
void Shoot()
{
    // 360°を等分
    float angleStep = 360f / spreadCount;

    for (int i = 0; i < spreadCount; i++)
    {
        // 通常の度数法の角度（0°=右, 90°=上, 180°=左, 270°=下）
        float angle = spreadAngle + angleStep * i;

        float rad = angle * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;

        // 弾生成
        var bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bulletObj.GetComponent<Bullet>().SetDirection(dir);
    }
}
}
