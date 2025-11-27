using UnityEngine;

[CreateAssetMenu(menuName = "Wave/MiniBossConfig", fileName = "MiniBossConfig")]
public class MiniBossConfig : ScriptableObject
{
    [Header("MiniBosses to Spawn")]
    public MiniBossController[] miniBossPrefabs;

    [Header("Spawn Settings")]
    public float spawnDistanceMin = 5f;
    public float spawnDistanceMax = 10f;

    [Header("Spawn Count")]
    public int spawnCount = 1;   // 1体〜複数体に対応
}
