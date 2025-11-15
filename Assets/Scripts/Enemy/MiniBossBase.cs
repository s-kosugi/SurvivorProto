using UnityEngine;

public class MiniBossBase : MonoBehaviour
{
    public MiniBossHealth Health { get; private set; }

    protected virtual void Awake()
    {
        // HP管理を取得
        Health = GetComponent<MiniBossHealth>();

        if (Health == null)
        {
            Debug.LogError($"{name} に MiniBossHealth がアタッチされていません！");
        }
    }

    // 派生ボス（近距離/遠距離）がAIを実装するための仮想関数
    protected virtual void Start() { }
    protected virtual void Update() { }
}
