using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField]public int maxHP = 3;
    [SerializeField]public int score = 10;
    private int currentHP;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.Instance?.AddScore(score);
        Destroy(gameObject);
        // TODO: 将来的に死亡エフェクトやスコア加算をここに追加
    }
}