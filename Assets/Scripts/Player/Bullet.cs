using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;
    public int damage = 1;

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
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(damage,AttackType.Bullet,this.transform.position);
            Destroy(gameObject);
        }
    }
}
