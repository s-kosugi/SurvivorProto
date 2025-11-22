using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField]public int maxHP = 3;
    [SerializeField]public int score = 10;
    [SerializeField]EnemyDropper dropper;
    [SerializeField]SpriteRenderer enemyRenderer;

    private int currentHP;
    private EffectLibrary effectLibrary;

    protected virtual void Start()
    {
        currentHP = maxHP;
        effectLibrary = EffectLibrary.Instance;
    }

    public void TakeDamage(int damage,AttackType attackType,Vector3 attackerPos)
    {
        EffectType effectType = EffectType.None;
        currentHP -= damage;

        // 攻撃タイプに応じたエフェクト生成
        effectType = EffectLibrary.Instance.GetDamageEffectType(attackType);

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

    protected virtual void Die()
    {
        // アイテムドロップ処理
        if (dropper != null)
            dropper.ExecuteDrop(transform.position);

        GameManager.Instance?.AddScore(score);
        EnemyManager.Instance.UnregisterEnemy(this.gameObject);
        Destroy(gameObject);
    }

}