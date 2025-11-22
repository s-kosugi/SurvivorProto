using UnityEngine;

public class PlayerCore : MonoBehaviour
{
    public Transform body;  // プレイヤーの Transform
    public PlayerExpCollector expCollector;
    public PlayerHealth health;

    private void Awake()
    {
        Debug.Log("PlayerCore Awake");
        PlayerManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        if (PlayerManager.Instance != null)
            PlayerManager.Instance.Unregister(this);
    }
}
