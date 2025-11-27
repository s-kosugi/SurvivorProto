using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }
    private readonly List<GameObject> activeEnemies = new();
    public int TotalEnemyCount => activeEnemies.Count;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// 敵一覧登録
    /// </summary>
    /// <param name="enemy"></param>
    public void RegisterEnemy(GameObject enemy)
    {
        if (!activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
        }
    }

    /// <summary>
    /// 敵をリストから削除
    /// </summary>
    public void UnregisterEnemy(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }

    /// <summary>
    /// すべての敵を削除（リスタート・シーンリセット用）
    /// </summary>
    public void ClearAllEnemies()
    {
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        activeEnemies.Clear();
    }

    /// <summary>
    /// 現在生きている敵一覧を返す
    /// </summary>
    public IReadOnlyList<GameObject> GetActiveEnemies() => activeEnemies;
}
