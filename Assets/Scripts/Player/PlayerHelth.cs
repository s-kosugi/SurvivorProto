using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHP = 5;
    private int currentHP;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log($"Player took {damage} damage! HP: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player Died!");
        // TODO: ゲームオーバー画面やリスタート処理を追加
        // とりあえず一旦停止する
        Time.timeScale = 0f;
    }
}
