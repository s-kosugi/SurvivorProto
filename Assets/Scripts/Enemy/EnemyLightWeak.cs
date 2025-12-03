using UnityEngine;
public class EnemyLightWeak : MonoBehaviour
{
    [SerializeField] EnemyBase health;

    [SerializeField] private float lightRangedMultiplier = 3.0f;
    [SerializeField] private float darkMeleeMultiplier = 1f;
    [SerializeField] private float meleeHardness = 0.8f;
    [SerializeField] private float knockbackPower = 0.2f;
    [SerializeField] private EnemyBase enemyBase;

    public void ApplyWeaknessDamage(int dmg, PlayerModeState form, AttackType type, Vector2 hitDir)
    {
        float finalDamage = dmg;

        if (form == PlayerModeState.Light && type == AttackType.Bullet)
        {
            finalDamage *= lightRangedMultiplier;
            transform.position += (Vector3)(hitDir * knockbackPower);
        }
        else if (form == PlayerModeState.Light && type == AttackType.Melee)
        {
            finalDamage *= meleeHardness;
        }
        else if (form == PlayerModeState.Dark)
        {
            finalDamage *= darkMeleeMultiplier;
        }

        health.TakeDamage(enemyBase.EnemyId,Mathf.RoundToInt(finalDamage), type, transform.position);
    }
}