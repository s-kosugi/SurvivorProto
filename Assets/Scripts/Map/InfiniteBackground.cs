using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    public Transform target;     // プレイヤーまたはメインカメラ
    public float tileWidth = 19.2f;  // 1920px / 100PPU
    public float tileHeight = 10.8f; // 1080px / 100PPU

    private Vector3 previousTargetPos;

    void Start()
    {
        previousTargetPos = target.position;
    }

    void LateUpdate()
    {
        // タイル単位で背景の中心を移動させる
        float x = Mathf.Round(target.position.x / tileWidth) * tileWidth;
        float y = Mathf.Round(target.position.y / tileHeight) * tileHeight;

        transform.position = new Vector3(x, y, transform.position.z);
    }
}
