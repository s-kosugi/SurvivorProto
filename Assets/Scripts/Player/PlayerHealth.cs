using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHP = 5;
    private int currentHP;

    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;

    public delegate void OnHealthChanged(int current, int max);
    public event OnHealthChanged HealthChanged;

    void Start()
    {
        currentHP = maxHP;
        HealthChanged?.Invoke(currentHP, maxHP); // 初期値反映
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        HealthChanged?.Invoke(currentHP, maxHP);
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
