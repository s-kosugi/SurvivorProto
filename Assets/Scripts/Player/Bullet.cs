using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("----- Basic Settings -----")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private int damage = 1;

    [Header("----- Visual -----")]
    [SerializeField] private bool enableGhost = true;
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private float ghostInterval = 0.04f;
    float ghostTimer = 0f;

    Vector2 _dir = Vector2.up;
    BulletOwner owner = BulletOwner.Player;

    // --------------------------------------------------------
    // 初期化（生成時に呼ぶ）
    // --------------------------------------------------------
    public void Init(Vector2 dir, BulletOwner owner, int damage)
    {
        this.owner = owner;
        this.damage = damage;

        SetDirection(dir);
    }

    public void SetDirection(Vector2 dir)
    {
        // 方向設定
        _dir = dir.sqrMagnitude > 0 ? dir.normalized : Vector2.up;

        // 回転設定
        float ang = Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, ang);
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(_dir * speed * Time.deltaTime, Space.World);

        // ---------------------------
        // 残像（player専用）
        // ---------------------------
        if (enableGhost && ghostPrefab != null)
        {
            ghostTimer += Time.deltaTime;
            if (ghostTimer >= ghostInterval)
            {
                ghostTimer = 0f;
                Instantiate(ghostPrefab, transform.position, transform.rotation);
            }
        }
    }

    // --------------------------------------------------------
    // 衝突処理
    // --------------------------------------------------------
    void OnTriggerEnter2D(Collider2D other)
    {
        // ===== プレイヤー弾 =====
        if (owner == BulletOwner.Player)
        {
            // 1) 光弱点の敵
            if (other.TryGetComponent(out EnemyLightWeak lightWeak))
            {
                Vector2 hitDir = (other.transform.position - transform.position).normalized;

                lightWeak.ApplyWeaknessDamage(
                    damage,
                    PlayerModeState.Light,
                    AttackType.Bullet,
                    hitDir
                );

                Destroy(gameObject);
                return;
            }

            // 2) 近接弱点敵（EnemyDarkWeak）対応
            if (other.TryGetComponent(out EnemyDarkWeak darkWeak))
            {
                Vector2 hitDir = (other.transform.position - transform.position).normalized;

                // カウンター発動
                darkWeak.OnHitByBullet(hitDir);

                // 弱点判定込みダメージ（闇/光フォームで可変）
                darkWeak.ApplyWeaknessDamage(
                    damage,
                    PlayerModeState.Light,
                    AttackType.Bullet,
                    hitDir
                );

                Destroy(gameObject);
                return;
            }

            // 3) 通常IDamageable
            if (other.TryGetComponent(out IDamageable target))
            {
                target.TakeDamage(damage, AttackType.Bullet, transform.position);
                Destroy(gameObject);
                return;
            }
        }

        // ===== 敵弾 =====
        else if (owner == BulletOwner.Enemy)
        {
            if (other.TryGetComponent(out PlayerHealth player))
            {
                player.TakeDamage(damage, AttackType.Bullet, transform.position);
                Destroy(gameObject);
                return;
            }
        }
    }
}
