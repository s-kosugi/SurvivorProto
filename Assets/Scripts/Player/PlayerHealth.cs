using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] PlayerExpCollector expCollector;

    [Header("----- HP Settings -----")]
    [SerializeField] private PlayerGrowthConfig growthConfig;
    [SerializeField] private int maxHP = 5;
    private int currentHP;

    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;

    [Header("----- Invincible Settings -----")]
    [SerializeField] private float invincibleTime = 1.0f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool isInvincible = false;
    private float invincibleTimer = 0f;

    // HP変更イベント
    public delegate void OnHealthChanged(int current, int max);
    public event OnHealthChanged HealthChanged;

    // 死亡イベント
    public delegate void OnDeath();
    public event OnDeath DeathEvent;

    void Start()
    {
        currentHP = maxHP;
        HealthChanged?.Invoke(currentHP, maxHP);
    }

    void Update()
    {
        // 無敵中の点滅処理
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;

            // 0.1秒ごとに表示/非表示切替（分かりやすい点滅）
            float blink = Mathf.Floor(invincibleTimer * 10f) % 2;
            spriteRenderer.enabled = (blink == 0);

            // 無敵終了
            if (invincibleTimer <= 0f)
            {
                isInvincible = false;
                spriteRenderer.enabled = true; // 表示に戻す
            }
        }
    }

    // ======================================================
    //  ダメージ処理
    // ======================================================
    public void TakeDamage(int damage, AttackType attackType, Vector3 attackerPos)
    {
        // 無敵中はダメージ無効
        if (isInvincible) return;

        currentHP -= damage;
        HealthChanged?.Invoke(currentHP, maxHP);

        // 無敵スタート
        isInvincible = true;
        invincibleTimer = invincibleTime;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    // ======================================================
    //  回復処理
    // ======================================================
    public void RecoverHP(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;

        SoundManager.Instance.PlaySE("HealItem");
        HealthChanged?.Invoke(currentHP, maxHP);
    }
    /// <summary>
    /// 最大HP設定
    /// </summary>
    public void RecalculateMaxHP(int lightLevel, int darkLevel)
    {
        int newMax = growthConfig.baseHP;

        // Light側の成長判定
        foreach (int lv in growthConfig.lightHpUpLevels)
        {
            if (lightLevel >= lv)
            {
                newMax += growthConfig.hpIncreaseAmount;
            }
        }

        // Dark側の成長判定
        foreach (int lv in growthConfig.darkHpUpLevels)
        {
            if (darkLevel >= lv)
            {
                newMax += growthConfig.hpIncreaseAmount;
            }
        }

        // 適用
        maxHP = newMax;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        HealthChanged?.Invoke(currentHP, maxHP);
    }

    // ======================================================
    //  死亡処理
    // ======================================================
    void Die()
    {
        // 先にイベント通知
        DeathEvent?.Invoke();

        // デスペナルティ適用
        expCollector.ApplyDeathPenalty();

        // ゲーム側終了処理（必要なら残す）
        GameManager.Instance?.EndGame();
    }

    // ======================================================
    //  HPリセット（ゲーム再スタート用）
    // ======================================================
    public void ResetHealth()
    {
        currentHP = maxHP;
        HealthChanged?.Invoke(currentHP, maxHP);

        // 無敵解除＆点滅リセット
        isInvincible = false;
        invincibleTimer = 0f;
        spriteRenderer.enabled = true;
    }
}
