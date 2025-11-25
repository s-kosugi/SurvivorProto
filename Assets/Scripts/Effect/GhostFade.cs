using UnityEngine;

public class GhostFade : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    public float life = 0.2f;
    private float timer = 0f;

    private void Update()
    {
        // ゲームがプレイ状態でなければできない
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
            return;
        timer += Time.deltaTime;
        float t = timer / life;

        // フェードアウト
        var c = sr.color;
        c.a = 1f - t;
        sr.color = c;

        if (timer >= life)
            Destroy(gameObject);
    }
}
