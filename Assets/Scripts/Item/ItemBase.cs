using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    [Header("Item Settings")]
    public float lifeTime = 20f;   // 20秒で消えるなど

    protected virtual void Start()
    {
        if (lifeTime > 0)
            Destroy(gameObject, lifeTime);
    }

    // プレイヤーが拾ったときの処理（各アイテムで実装）
    public abstract void Collect(PlayerCore player);
}
