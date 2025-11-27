using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable
{
    [SerializeField]public int maxHP = 3;
    [SerializeField]public int score = 10;
    [SerializeField]EnemyDropper dropper;
    [SerializeField]EnemyHitEffect enemyHitEffect;

    private int currentHP;

    protected virtual void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage,AttackType attackType,Vector3 attackerPos)
    {
        currentHP -= damage;

        // 攻撃タイプに応じたエフェクト生成
        enemyHitEffect.PlayHitEffect(attackType,attackerPos);

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