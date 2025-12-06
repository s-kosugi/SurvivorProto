using UnityEngine;

public class RoperEnemy : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private GameObject tentaclePrefab;  // RoperTentacle プレハブ
    [SerializeField] private float attackInterval = 2.0f; // 攻撃間隔
    [SerializeField] private float attackRange = 6.0f;    // 攻撃射程

    private Transform player;
    private float timer = 0f;

    void Start()
    {
        player = PlayerManager.Instance.MainPlayer.transform;
    }

    void Update()
    {
        if (GameManager.Instance == null ||
            GameManager.Instance.State != GameState.Playing)
            return;

        if (player == null) return;

        timer += Time.deltaTime;

        // 一定距離以内なら攻撃可能
        float dist = Vector2.Distance(transform.position, player.position);

        if (timer >= attackInterval && dist <= attackRange)
        {
            ShootTentacle();
            timer = 0f;
        }
    }

    private void ShootTentacle()
    {
        if (tentaclePrefab == null) return;

        // プレイヤーのコライダーの下端座標を取得
        Collider2D col = player.GetComponent<Collider2D>();
        Vector3 spawnPos = player.position;

        if (col != null)
        {
            // プレイヤーの中心 → コライダーの下端に変換
            float bottomY = col.bounds.min.y;
            spawnPos.y = bottomY;
        }
        else
        {
            // 念のため fallback
            spawnPos.y -= 0.2f;
        }
        SoundManager.Instance.PlaySE("TentacleAttack");
        Instantiate(tentaclePrefab, spawnPos, Quaternion.identity);
    }

}
