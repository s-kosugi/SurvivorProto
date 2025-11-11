using UnityEngine;

[DefaultExecutionOrder(-100)] // 先に初期化されるように
public class EffectLibrary : MonoBehaviour
{
    public static EffectLibrary Instance { get; private set; }

    [SerializeField] private GameObject meleeHitEffectPrefab;
    [SerializeField] private GameObject bulletHitEffectPrefab;
    [SerializeField] private GameObject slashEffectPrefab;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void SpawnEffect(EffectType type, Vector3 position, Quaternion rotation = default,Renderer targetRenderer = null,int sortingOffset = 1)
    {
        GameObject prefab = type switch
        {
            EffectType.MeleeHit => meleeHitEffectPrefab,
            EffectType.ShotHit => bulletHitEffectPrefab,
            EffectType.Slash => slashEffectPrefab,
            _ => null
        };

        if (prefab == null) return;

        GameObject fx = Instantiate(prefab, position, rotation);

        // 描画順の調整（Renderer指定ありの場合のみ）
        if (targetRenderer != null)
        {
            var psRenderer = fx.GetComponentInChildren<ParticleSystemRenderer>();
            if (psRenderer != null)
            {
                psRenderer.sortingLayerID = targetRenderer.sortingLayerID;
                psRenderer.sortingOrder   = targetRenderer.sortingOrder + sortingOffset;
            }
        }
    }
}
