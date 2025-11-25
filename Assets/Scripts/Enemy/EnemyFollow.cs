using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyFollow : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] Rigidbody2D rb;
    private Transform player;

    void Start()
    {
        // PlayerPlayerObjectを取得
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

}
