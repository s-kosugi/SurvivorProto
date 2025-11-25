using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MiniBossMelee : MonoBehaviour
{
    [Header("----- References -----")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private EnemyPhysicalFollow followAI;
    [SerializeField] private Rigidbody2D rb;

    private Transform player;

    [Header("----- Follow Settings -----")]
    [SerializeField] private float triggerDistance = 6.0f; // この距離に入ると突進行動へ移行

    [Header("----- Dash Settings -----")]
    [SerializeField] private float chargeTime = 0.6f;     // 溜め時間
    [SerializeField] private float dashSpeed = 14f;       // 突進速度
    [SerializeField] private float dashDuration = 0.5f;   // 突進時間
    [SerializeField] private float recoverTime = 0.8f;    // 回復（硬直）時間

    [Header("----- PowerUp (Counter Reaction) -----")]
    [SerializeField] private float chargeTimeMultiplier = 0.85f;    // 溜め短縮率
    [SerializeField] private float recoverShortenMultiplier = 0.85f; // Recover後の追尾復帰短縮


    private float stateTimer = 0f;

    private enum BossState
    {
        Follow,     // 通常追尾
        Charge,     // 溜め
        Dash,       // 突進
        Recover     // 硬直（追尾復帰前）
    }

    private BossState current = BossState.Follow;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (followAI == null) followAI = GetComponent<EnemyPhysicalFollow>();
    }

    void Start()
    {
        player = PlayerManager.Instance.MainPlayer.transform;
    }

    void Update()
    {
        // ゲームがプレイ状態でなければできない
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
            return;

        if (player == null) return;

        stateTimer += Time.deltaTime;

        switch (current)
        {
            case BossState.Follow:
                followAI.EnableFollow = true;
                UpdateFollow();
                break;

            case BossState.Charge:
                followAI.EnableFollow = false;
                if (stateTimer >= chargeTime)
                    StartDash();
                break;

            case BossState.Dash:
                followAI.EnableFollow = false;
                if (stateTimer >= dashDuration)
                    EndDash();
                break;

            case BossState.Recover:
                followAI.EnableFollow = false;
                if (stateTimer >= recoverTime)
                    ChangeState(BossState.Follow);
                break;
        }
    }

    // ----------------------------------------------------------
    // Follow（通常追尾）
    // ----------------------------------------------------------
    private void UpdateFollow()
    {
        float dist = Vector2.Distance(transform.position, player.position);

        // プレイヤーが近すぎたら突進開始
        if (dist < triggerDistance)
        {
            StartCharge();
            return;
        }

        // 追尾ON
        followAI.EnableFollow = true;
    }

    // ----------------------------------------------------------
    // Charge 開始
    // ----------------------------------------------------------
    private void StartCharge()
    {
        ChangeState(BossState.Charge);

        // 追尾停止
        followAI.EnableFollow = false;
        rb.velocity = Vector2.zero;

        // 溜め時の色変化
        sprite.color = new Color(1.0f, 0.7f, 0.7f);
    }

    // ----------------------------------------------------------
    // Dash 開始
    // ----------------------------------------------------------
    private void StartDash()
    {
        ChangeState(BossState.Dash);

        sprite.color = Color.white;

        Vector2 dir = (player.position - transform.position).normalized;

        // 物理突進
        rb.velocity = dir * dashSpeed;
    }

    // ----------------------------------------------------------
    // Dash 終了
    // ----------------------------------------------------------
    private void EndDash()
    {
        rb.velocity = Vector2.zero;
        ChangeState(BossState.Recover);
    }

    // ----------------------------------------------------------
    // 状態変更
    // ----------------------------------------------------------
    private void ChangeState(BossState next)
    {
        current = next;
        stateTimer = 0f;
    }

    // ----------------------------------------------------------
    // カウンターによる突進キャンセル用
    // ----------------------------------------------------------
   public void InterruptDash()
    {
        if (current != BossState.Dash) return;

        rb.velocity = Vector2.zero;

        // ひるみ演出
        sprite.color = Color.yellow;

        // ★ 次回チャージ時間を短縮 ★
        chargeTime *= chargeTimeMultiplier;

        // ★ Recover → Follow に戻るまでの時間も短縮（Optional）★
        recoverTime *= recoverShortenMultiplier;

        Debug.Log($"[MiniBossMelee] Counter! 次回: Charge={chargeTime:F2}, Recover={recoverTime:F2}");

        ChangeState(BossState.Recover);
    }

}
