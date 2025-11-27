using UnityEngine;

[CreateAssetMenu(menuName = "Game/WaveProfile", fileName = "WaveProfile")]
public class WaveProfile : ScriptableObject
{
    [Header("Wave Configs (Each Wave Pattern)")]
    public WaveConfig[] waveConfigs;

    [Header("Timing Settings (WaveDuration & BossTime)")]
    public WaveTimingSet timing;

    [Header("MiniBoss Settings")]
    public MiniBossConfigSet miniBossConfigSet;
}
