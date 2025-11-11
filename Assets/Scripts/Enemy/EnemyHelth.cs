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

    public void TakeDamage(int damage,AttackType attackType = AttackType.Melee)
    {
        EffectType effectType = EffectType.None;
        Debug.Log($"TakeDamage called by {new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().DeclaringType}");
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

        effectLibrary.SpawnEffect(effectType,transform.position,default,enemyRenderer);

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