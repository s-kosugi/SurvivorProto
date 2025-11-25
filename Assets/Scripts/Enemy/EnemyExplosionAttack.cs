using UnityEngine;

public class EnemyExplosionAttack : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionRange = 2.5f;
    public int explosionDamage = 2;
    public float chargeTime = 0.8f;
    public float flashInterval = 0.1f;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private SpriteRenderer enemyRenderer;
    [SerializeField] private GameObject explosionIndicator; // 子の円スプライト

    bool charging = false;
    float chargeTimer = 0f;
    float flashTimer = 0f;
    bool flashToggle = false;

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
                    explosionDamage,
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
}
