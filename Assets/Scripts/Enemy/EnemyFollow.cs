using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyBase))]
public class EnemyFollow : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] EnemyBase enemyBase;
    private Transform player;
    float moveSpeed = 2f;

    void Awake()
    {
        enemyBase.OnBalanceApplied += ApplyBalance;
    }
    void Start()
    {
        // PlayerObjectを取得
        GameObject playerObj = PlayerManager.Instance.MainPlayer.gameObject;

        if (playerObj != null)
            player = playerObj.transform;
    }

    void FixedUpdate()
    {
        // ゲームがプレイ状態でなければできない
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
            return;

        if (player == null) return;

        // プレイヤーへの方向
        Vector2 direction = ((Vector2)player.position - rb.position).normalized;

        // スプライト反転
        if (direction.x != 0)
        {
            if (spriteRenderer != null)
                spriteRenderer.flipX = direction.x < 0; // 左に向いていたら反転
        }

        // スムーズ移動
        Vector2 newPos = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
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
