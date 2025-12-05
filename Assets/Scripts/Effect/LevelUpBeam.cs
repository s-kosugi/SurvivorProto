using UnityEngine;

public class LevelUpBeam : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    public float growTime = 0.2f;  // 伸びる時間
    public float fadeTime = 0.2f;  // 消える時間

    private float timer = 0f;
    private bool growing = true;

    public void SetColor(Color c)
    {
        sr.color = c;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (growing)
        {
            float t = timer / growTime;
            float scaleY = Mathf.Lerp(0.1f, 1.4f, t);
            transform.localScale = new Vector3(1f, scaleY, 1f);

            if (t >= 1f)
            {
                // 次はフェードアウトに切り替え
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

            if (t >= 1f)
            {
                Destroy(gameObject);
            }
        }
    }
}
