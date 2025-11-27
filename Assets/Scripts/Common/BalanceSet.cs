using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BalanceSet", menuName = "Game/Balance Set")]
public class BalanceSet : ScriptableObject
{
    [Header("Enemy Stats")]
    public List<EnemyStat> enemyStats;

    public EnemyStat GetStat(EnemyID id)
    {
        return enemyStats.Find(s => s.id == id);
    }
}

[System.Serializable]
public class EnemyStat
{
    public EnemyID id;
    public int maxHP = 3;
    public int attack = 1;
    public float moveSpeed = 2f;
    public int score = 10;
}
