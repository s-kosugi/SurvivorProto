using UnityEngine;

public interface IDamageable
{
    void TakeDamage(EnemyID enemyID,int damage,AttackType attackType,Vector3 attackerPos);
}