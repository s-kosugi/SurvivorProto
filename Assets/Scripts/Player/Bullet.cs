using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("----- Basic Settings -----")]
    [SerializeField] private float speed = 10f;     // Initで上書き可能
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private int damage = 1;

    [Header("----- Visual -----")]
    [SerializeField] private bool enableGhost = true;
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private float ghostInterval = 0.04f;
    float ghostTimer = 0f;

    private Vector2 _dir = Vector2.up;
    private BulletOwner owner = BulletOwner.Player;

    // ========================================================
    // ★★ Initを拡張して速度まで全セット可能にする ★★
    // ========================================================
    public void Init(Vector2 dir, BulletOwner owner, int damage, float speed = 10f)
    {
        this.owner = owner;
        this.damage = damage;
        this.speed = speed;

        SetDirection(dir);
    }

    // 任意で単独変更したい場合用の補助関数（残す）
    public void SetSpeed(float newSpeed) => speed = newSpeed;

    public void SetDirection(Vector2 dir)
    {
        // 方向設定
        _dir = dir.sqrMagnitude > 0 ? dir.normalized : Vector2.up;

        // 弾の見た目の回転設定
        float ang = Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, ang);
    }

    void Start()
    {
        BulletManager.Instance?.RegisterBullet(this.gameObject);
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
        if (owner == BulletOwner.Player)
        {
            // 光弱点
            if (other.TryGetComponent(out EnemyLightWeak lightWeak))
            {
                Vector2 hitDir = (other.transform.position - transform.position).normalized;

                lightWeak.ApplyWeaknessDamage(damage, PlayerModeState.Light,
                                              AttackType.Bullet, hitDir);

                Destroy(gameObject);
                return;
            }

            // 闇弱点
            if (other.TryGetComponent(out EnemyDarkWeak darkWeak))
            {
                Vector2 hitDir = (other.transform.position - transform.position).normalized;

                darkWeak.OnHitByBullet(hitDir);
                darkWeak.ApplyWeaknessDamage(damage, PlayerModeState.Light,
                                             AttackType.Bullet, hitDir);

                Destroy(gameObject);
                return;
            }

            // 通常IDamageable
            if (other.TryGetComponent(out IDamageable target))
            {
                target.TakeDamage(damage, AttackType.Bullet, transform.position);
                Destroy(gameObject);
                return;
            }
        }
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
    private void OnDestroy()
    {
        BulletManager.Instance?.UnregisterBullet(this.gameObject);
    }
}
