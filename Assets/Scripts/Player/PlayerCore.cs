using UnityEngine;

public class PlayerCore : MonoBehaviour
{
    public Transform body;  // プレイヤーの Transform
    public PlayerExpCollector expCollector;
    public PlayerHealth health;
    // 攻撃ステータス
    public PlayerAttackStats attackStats { get; private set; }

    private void Awake()
    {
        Debug.Log("PlayerCore Awake");
        attackStats = new PlayerAttackStats();
        PlayerManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        if (PlayerManager.Instance != null)
            PlayerManager.Instance.Unregister(this);
    }
}
