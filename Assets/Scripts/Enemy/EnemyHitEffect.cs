using UnityEngine;

/// <summary>
/// 敵攻撃時ヒットエフェクト
/// </summary>
public class EnemyHitEffect : MonoBehaviour
{
    [SerializeField] SpriteRenderer enemyRenderer;

    public void PlayHitEffect(AttackType attackType, Vector3 attackerPos)
    {
        var effectType = EffectLibrary.Instance.GetDamageEffectType(attackType);

        if (attackType == AttackType.Melee)
        {
            SoundManager.Instance.PlaySE("MeleeHit");
        }

        Vector3 center = (transform.position + attackerPos) * 0.5f;
        Vector3 dir = (transform.position - attackerPos).normalized;
        Vector3 hitPos = center - dir * 0.2f;

        EffectLibrary.Instance.SpawnEffect(effectType, hitPos, Quaternion.identity, enemyRenderer, 1);
    }
}
