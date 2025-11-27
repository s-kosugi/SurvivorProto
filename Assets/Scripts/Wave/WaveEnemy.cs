using UnityEngine;

[System.Serializable]
public class WaveEnemy
{
    [Header("Enemy Settings")]
    public EnemyID enemyId;  // 出現する敵のID
    public int count = 1;      // 出す数
    public float interval = 0.3f; // 連続出現させる時の間隔
}
