using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int damage,AttackType attackType,Vector3 attackerPos);
}