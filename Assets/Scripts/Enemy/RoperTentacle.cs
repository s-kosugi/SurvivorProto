using UnityEngine;

public class RoperTentacle : MonoBehaviour
{
    [SerializeField]private Collider2D col;
    [Header("Animation")]
    [SerializeField] private float riseTime = 0.25f;     
    [SerializeField] private float holdTime = 0.1f;      
    [SerializeField] private float returnTime = 0.25f;   
    [SerializeField] private float attackTriggerScaleY = 0.5f;

    [Header("Damage Settings")]
    [SerializeField] private int damage = 1;
    [SerializeField] private EnemyID enemyID = EnemyID.Dummy;

    private Vector3 originalScale;
    private bool attackActivated = false;
    private bool attackTriggered = false;

    private void Awake()
    {
        col.enabled = false;

        originalScale = transform.localScale;
        // 最初は地面に埋まっているので scaleY = 0
        transform.localScale = new Vector3(originalScale.x, 0f, originalScale.z);
    }

    private void Start()
    {
        // ★ BulletManager に登録（敵弾扱い）
        BulletManager.Instance?.RegisterBullet(this.gameObject);

        // アニメーション開始
        StartCoroutine(Flow());
    }

    private void OnDestroy()
    {
        BulletManager.Instance?.UnregisterBullet(this.gameObject);
    }

    private System.Collections.IEnumerator Flow()
    {
        // 1) 出現フェーズ（0 → 1）
        float t = 0f;
        while (t < riseTime)
        {
            t += Time.deltaTime;
            float r = t / riseTime;

            float newY = Mathf.Lerp(0f, originalScale.y, r);
            transform.localScale = new Vector3(originalScale.x, newY, originalScale.z);

            // 攻撃判定 ON（ScaleY がしきい値超えた瞬間）
            if (!attackTriggered && r >= attackTriggerScaleY)
            {
                col.enabled = true;
                attackActivated = true;
                attackTriggered = true;
            }

            yield return null;
        }

        // 2) 最大まで伸びた状態で少し待つ
        yield return new WaitForSeconds(holdTime);

        // 3) 消失フェーズ（1 → 0）
        t = 0f;
        while (t < returnTime)
        {
            t += Time.deltaTime;
            float r = t / returnTime;

            float newY = Mathf.Lerp(originalScale.y, 0f, r);
            transform.localScale = new Vector3(originalScale.x, newY, originalScale.z);

            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!attackActivated) return;

        if (other.TryGetComponent(out PlayerHealth player))
        {
            SoundManager.Instance.PlaySE("ShotDamage");
            player.TakeDamage(enemyID, damage, AttackType.Bullet, transform.position);
        }
    }
}
