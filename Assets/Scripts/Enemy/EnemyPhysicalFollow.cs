using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyBase))]
public class EnemyPhysicalFollow : MonoBehaviour
{
    [Header("----- Follow Settings -----")]
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private float stopDistance = 1.5f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] EnemyBase enemyBase;

    private Transform player;

    public bool EnableFollow { get; set; } = true;

    void Awake()
    {
        enemyBase.OnBalanceApplied += ApplyBalance;
    }

    void Start()
    {
        // Player位置取得
        player = PlayerManager.Instance.MainPlayer.transform;
    }

    void FixedUpdate()
    {
        // ゲームがプレイ状態でなければできない
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
            return;
        if (!EnableFollow) return;
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist < stopDistance)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        Vector2 dir = (player.position - transform.position).normalized;

        // スプライト反転
        if (dir.x != 0)
        {
            if (spriteRenderer != null)
                spriteRenderer.flipX = dir.x < 0;
        }

        rb.velocity = dir * moveSpeed;
    }
    /// <summary>
    /// バランス適用
    /// </summary>
    /// <param name="stat"></param>
    private void ApplyBalance(EnemyStat stat)
    {
        moveSpeed = stat.moveSpeed;
    }

}
