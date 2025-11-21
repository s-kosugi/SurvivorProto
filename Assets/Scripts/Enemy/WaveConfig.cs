using UnityEngine;

[CreateAssetMenu(menuName = "Wave/WaveConfig", fileName = "WaveConfig")]
public class WaveConfig : ScriptableObject
{
    [Header("Enemies in this Wave")]
    public WaveEnemy[] enemies;  // このWave内で出す敵の一覧
}
