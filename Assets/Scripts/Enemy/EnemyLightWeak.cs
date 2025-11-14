using UnityEngine;
public class EnemyLightWeak : MonoBehaviour
{
    [SerializeField] EnemyHealth health;

    public float lightRangedMultiplier = 1.8f;
    public float darkMeleeMultiplier = 1f;
    public float meleeHardness = 0.8f;
    public float knockbackPower = 0.2f;

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

        health.TakeDamage(Mathf.RoundToInt(finalDamage), type, transform.position);
    }
}