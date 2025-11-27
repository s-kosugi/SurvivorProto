using UnityEngine;

[RequireComponent(typeof(EnemyBase))]
public class EnemyExplosionAttack : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float explosionRange = 2.5f;
    [SerializeField] private int explosionDamageRate = 2;
    [SerializeField] private float chargeTime = 0.8f;
    [SerializeField] private float flashInterval = 0.1f;

    [Header("References")]
    [SerializeField] private SpriteRenderer enemyRenderer;
    [SerializeField] private GameObject explosionIndicator; // 子の円スプライト
    [SerializeField] private EnemyBase enemyBase;

    private bool charging = false;
    private float chargeTimer = 0f;
    private float flashTimer = 0f;
    private bool flashToggle = false;
    private int baseDamage = 1;
    private Transform player;

    void Awake()
    {
        enemyBase.OnBalanceApplied += ApplyBalance;
    }
    private void Start()
    {
        // プレイヤー取得
        player = PlayerManager.Instance.MainPlayer.transform;

        if (explosionIndicator != null)
        {
            explosionIndicator.SetActive(false);
            // 半径を直接スケールに反映
            explosionIndicator.transform.localScale =
                Vector3.one * (explosionRange * 2f);
        }
    }

    private void Update()
    {
        // ゲームがプレイ状態でなければできない
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
            return;
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist < explosionRange)
        {
            StartChargeAttack();
        }
        else
        {
            ResetCharge();
        }
    }

    void StartChargeAttack()
    {
        charging = true;
        chargeTimer += Time.deltaTime;
        flashTimer += Time.deltaTime;

        if (explosionIndicator) explosionIndicator.SetActive(true);

        // 点滅
        if (flashTimer > flashInterval)
        {
            flashToggle = !flashToggle;
            enemyRenderer.color =
                flashToggle ? new Color(1f, 0.5f, 0.5f, 0.6f) : Color.white;
            flashTimer = 0f;
        }

        // 爆発処理
        if (chargeTimer >= chargeTime)
        {
            float currentDist = Vector2.Distance(transform.position, player.position);
            if (currentDist < explosionRange)
            {
                PlayerManager.Instance.MainPlayer.health?.TakeDamage(
                    explosionDamageRate * baseDamage,
                    AttackType.Explosion,
                    transform.position
                );
            }

            chargeTimer = 0f;
        }
    }

    void ResetCharge()
    {
        charging = false;
        chargeTimer = 0f;
        flashTimer = 0f;
        flashToggle = false;

        enemyRenderer.color = Color.white;
        if (explosionIndicator) explosionIndicator.SetActive(false);
    }
    /// <summary>
    /// バランス適用
    /// </summary>
    /// <param name="stat"></param>
    private void ApplyBalance(EnemyStat stat)
    {
        baseDamage = stat.attack;
    }
}
