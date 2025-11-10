using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float shootCooldown = 0.3f; // 連射間隔

    private float nextShootTime;

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && Time.time >= nextShootTime)
        {
            Debug.Log("test Shoot");
            Shoot();
            nextShootTime = Time.time + shootCooldown;
        }
    }

    void Shoot()
    {
        // プレイヤーの向いている方向に弾を撃つ
        Vector2 shootDir = transform.up;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().SetDirection(shootDir);
    }
}
