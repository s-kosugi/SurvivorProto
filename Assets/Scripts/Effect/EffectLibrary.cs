using UnityEngine;

[DefaultExecutionOrder(-100)] // 先に初期化されるように
public class EffectLibrary : MonoBehaviour
{
    public static EffectLibrary Instance { get; private set; }

    [SerializeField] private GameObject meleeHitEffectPrefab;
    [SerializeField] private GameObject bulletHitEffectPrefab;
    [SerializeField] private GameObject slashEffectPrefab;

    private Transform effectRoot;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        effectRoot = this.transform;
    }
    // 攻撃タイプからダメージエフェクトタイプを返す
    public EffectType GetDamageEffectType(AttackType attackType)
    {
        return attackType switch
        {
            AttackType.Melee => EffectType.MeleeHit,
            AttackType.Bullet => EffectType.ShotHit,
            _ => EffectType.None
        };
    }

    public void SpawnEffect(EffectType type, Vector3 position, Quaternion rotation = default, Renderer targetRenderer = null, int sortingOffset = 1)
    {
        GameObject prefab = type switch
        {
            EffectType.MeleeHit => meleeHitEffectPrefab,
            EffectType.ShotHit => bulletHitEffectPrefab,
            EffectType.Slash => slashEffectPrefab,
            _ => null
        };

        if (prefab == null) return;

        // エフェクトルート配下に生成する
        GameObject fx = Instantiate(prefab, position, rotation, effectRoot);

        // 描画順の調整（Renderer指定ありの場合のみ）
        if (targetRenderer != null)
        {
            var psRenderer = fx.GetComponentInChildren<ParticleSystemRenderer>();
            if (psRenderer != null)
            {
                psRenderer.sortingLayerID = targetRenderer.sortingLayerID;
                psRenderer.sortingOrder = targetRenderer.sortingOrder + sortingOffset;
            }
        }
    }
    /// <summary>
    /// エフェクト全クリア
    /// </summary>
    public void ClearAllEffects()
    {
        foreach (Transform child in effectRoot)
        {
            Destroy(child.gameObject);
        }
    }
}
