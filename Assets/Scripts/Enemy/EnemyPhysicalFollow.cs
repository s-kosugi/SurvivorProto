using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyPhysicalFollow : MonoBehaviour
{
    [Header("----- Follow Settings -----")]
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private float stopDistance = 1.5f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Transform player;

    public bool EnableFollow { get; set; } = true;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void FixedUpdate()
    {
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

}
