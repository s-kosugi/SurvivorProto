using System.Collections;
using UnityEngine;

public class ExpPickupPop : MonoBehaviour
{
    [Header("Pop Sprite")]
    [SerializeField] private SpriteRenderer popSprite;

    [Header("Animation Settings")]
    [SerializeField] private float duration = 0.12f;         // 全体の時間
    [SerializeField] private float popScale = 1.3f;          // 拡大倍率
    [SerializeField] private Color midColor = Color.white;   // ピーク時の色（白など）
    [SerializeField] private Color endColor = new Color(1,1,1,0); // 最終フェード色（透明）
    
    private Vector3 startScale = Vector3.one;

    private void OnEnable()
    {
        StartCoroutine(PopAnimation());
    }

    IEnumerator PopAnimation()
    {
        Vector3 endScale = Vector3.one * popScale;
        Color startColor = popSprite.color;

        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;

            // 色変化（前半：start→mid、後半：mid→end）
            if (p < 0.5f)
                popSprite.color = Color.Lerp(startColor, midColor, p * 2f);
            else
                popSprite.color = Color.Lerp(midColor, endColor, (p - 0.5f) * 2f);

            // 拡大
            popSprite.transform.localScale = Vector3.Lerp(startScale, endScale, p);

            yield return null;
        }

        popSprite.color = endColor;
        popSprite.transform.localScale = endScale;
    }
}
