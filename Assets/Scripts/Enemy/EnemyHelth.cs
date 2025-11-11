using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField]public int maxHP = 3;
    [SerializeField] public int score = 10;

    private int currentHP;
    private EffectLibrary effectLibrary;
    private SpriteRenderer enemyRenderer;

    void Start()
    {
        currentHP = maxHP;
        effectLibrary = EffectLibrary.Instance;
        enemyRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage,AttackType attackType,Vector3 attackerPos)
    {
        EffectType effectType = EffectType.None;
        currentHP -= damage;

        // ★ 攻撃タイプに応じたエフェクト生成
        switch (attackType)
        {
            case AttackType.Melee:
                effectType = EffectType.MeleeHit;
                break;
            case AttackType.Bullet:
                effectType = EffectType.ShotHit;
                break;
        }

        // 攻撃と敵の間にヒットエフェクトを生成
        Vector3 center = (transform.position + attackerPos) * 0.5f;
        Vector3 dir = (transform.position - attackerPos).normalized;
        Vector3 hitPos = center - dir * 0.2f;

        EffectLibrary.Instance.SpawnEffect(effectType, hitPos, Quaternion.identity, enemyRenderer, 1);


        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.Instance?.AddScore(score);
        Destroy(gameObject);
    }
}