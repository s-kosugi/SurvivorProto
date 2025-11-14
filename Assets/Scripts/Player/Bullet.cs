using UnityEngine;

public class Bullet : MonoBehaviour
{
    // 残像スプライト
    [SerializeField] private GameObject ghostPrefab;
    // 残像スプライトの間隔
    [SerializeField] private float ghostInterval = 0.04f;
    public float speed = 10f;
    public float lifetime = 2f;
    public int damage = 1;
    private float ghostTimer = 0f;

    Vector2 _dir = Vector2.up;  // デフォルト（上方向）

    public void SetDirection(Vector2 dir)
    {
        _dir = dir.sqrMagnitude > 0 ? dir.normalized : Vector2.up;
        // 見た目の向きも合わせたい場合（※2DなのでZ回転）
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

         // ---- 残像 ----
        if(ghostPrefab != null)
        {
            ghostTimer += Time.deltaTime;
            if (ghostTimer >= ghostInterval)
            {
                ghostTimer = 0f;
                var g = Instantiate(ghostPrefab, transform.position, transform.rotation);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out EnemyLightWeak weak))
        {
            // ★ 弱点処理版ダメージを優先
            weak.ApplyWeaknessDamage(
                damage,
                PlayerModeState.Light,         // ← 今のフォームを渡す
                AttackType.Bullet,
                (other.transform.position - transform.position).normalized
            );

            Destroy(gameObject);
            return;  // ★ EnemyHealth.TakeDamageを呼ばないように抜ける
        }
        
        if (other.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(damage,AttackType.Bullet,this.transform.position);
            Destroy(gameObject);
        }
    }
}
