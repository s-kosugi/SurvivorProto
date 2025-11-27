using UnityEngine;

[CreateAssetMenu(menuName = "Game/WaveTimingSet", fileName = "WaveTimingSet")]
public class WaveTimingSet : ScriptableObject
{
    [Header("Duration of Each Wave (seconds)")]
    public float waveDuration = 60f;

    [Header("Time Until MiniBoss Appears In A Wave (seconds)")]
    public float miniBossTime = 180f;
}
