using UnityEngine;

public class LevelUpBeamDark : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private float growTime = 0.2f;
    [SerializeField] private float fadeTime = 0.2f;

    private float timer = 0f;
    private bool growing = true;
    private Transform followTarget;
    private Vector3 offset;

    void Awake()
    {
        // 闇フォームのレベルアップ色
        sr.color = new Color(0.45f, 0.05f, 0.50f, 1f);
        // 初期スケール
        transform.localScale = new Vector3(2.3f, 0.1f, 1f);
    }
    void Start()
    {
        // ★ Startでプレイヤーを確実に取得（安全）
        followTarget = PlayerManager.Instance.MainPlayer.transform;

        // 足元に合わせたオフセット調整（ここを調整すると位置が決まる）
        offset = new Vector3(0f, -1.0f, 0f);
    }

    void Update()
    {
        if (growing && followTarget != null)
            transform.position = followTarget.position + offset;

        timer += Time.deltaTime;

        if (growing)
        {
            float t = timer / growTime;
            float scaleY = Mathf.Lerp(0.1f, 2.2f, t);
            transform.localScale = new Vector3(2.3f, scaleY, 1f);

            if (t >= 1f)
            {
                growing = false;
                timer = 0f;
            }
        }
        else
        {
            float t = timer / fadeTime;
            Color c = sr.color;
            c.a = Mathf.Lerp(1f, 0f, t);
            sr.color = c;

            if (t >= 1f) Destroy(gameObject);
        }
    }
}
