using UnityEngine;

public class EffectSelfDestroy : MonoBehaviour
{
    [SerializeField] private ParticleSystem targetParticle; // 寿命を基準にするパーティクル

    private void Start()
    {
        if (targetParticle == null)
        {
            Debug.LogWarning($"{name}: targetParticle not set! Defaulting to self.");
            targetParticle = GetComponent<ParticleSystem>(); // 念のためのフォールバック
        }

        if (targetParticle != null)
        {
            float duration = targetParticle.main.duration + targetParticle.main.startLifetime.constantMax;
            Destroy(gameObject, duration);
        }
        else
        {
            // 最低限の安全策（設定漏れ時）
            Destroy(gameObject, 5f);
        }
    }
}
