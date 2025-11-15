using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public static BulletManager Instance { get; private set; }

    private readonly List<GameObject> bullets = new List<GameObject>();

    void Awake()
    {
        Instance = this;
    }

    // 弾を登録
    public void RegisterBullet(GameObject bullet)
    {
        bullets.Add(bullet);
    }

    // 弾が消えるときに解除
    public void UnregisterBullet(GameObject bullet)
    {
        bullets.Remove(bullet);
    }

    // 全弾削除（ゲームオーバー・WaveClear用）
    public void ClearAllBullets()
    {
        foreach (var b in bullets)
        {
            if (b != null)
                Destroy(b);
        }
        bullets.Clear();
    }
}
