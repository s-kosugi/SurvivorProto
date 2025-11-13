using UnityEngine;

public class SwitchFlash : MonoBehaviour
{
    public float duration = 0.2f;
    public float startScale = 0.2f;
    public float endScale = 1.2f;

    private SpriteRenderer sr;
    private float timer;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        transform.localScale = Vector3.one * startScale;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = timer / duration;

        // 拡大
        float scale = Mathf.Lerp(startScale, endScale, t);
        transform.localScale = Vector3.one * scale;

        // フェードアウト
        var c = sr.color;
        c.a = 1f - t;
        sr.color = c;

        if (timer >= duration)
            Destroy(gameObject);
    }
}
