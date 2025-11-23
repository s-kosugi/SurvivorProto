using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    [Header("Item Settings")]
    public float lifeTime = 20f;

    protected virtual void Start()
    {
        // 自動登録（Manager存在チェック付き）
        if (ItemManager.Instance != null)
            ItemManager.Instance.Register(gameObject);

        // 寿命タイマー
        if (lifeTime > 0)
            Destroy(gameObject, lifeTime);
    }

    protected virtual void OnDestroy()
    {
        // 自動解除（Manager存在チェック）
        if (ItemManager.Instance != null)
            ItemManager.Instance.Unregister(gameObject);
    }

    // プレイヤーが拾ったときの処理（子クラスが実装）
    public abstract void Collect(PlayerCore player);
}
