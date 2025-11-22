using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    private readonly List<PlayerCore> players = new List<PlayerCore>();
    public IReadOnlyList<PlayerCore> Players => players;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Register(PlayerCore player)
    {
        if (!players.Contains(player))
            players.Add(player);
    }

    public void Unregister(PlayerCore player)
    {
        if (players.Contains(player))
            players.Remove(player);
    }

    // シングルプレイ向けショートカット
    public PlayerCore MainPlayer => players.Count > 0 ? players[0] : null;

    // マルチ対応：最も近いプレイヤーを取得する
    public PlayerCore GetNearestPlayer(Vector3 position)
    {
        PlayerCore nearest = null;
        float bestDist = float.MaxValue;

        foreach (var p in players)
        {
            float d = Vector3.Distance(position, p.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                nearest = p;
            }
        }

        return nearest;
    }
}
