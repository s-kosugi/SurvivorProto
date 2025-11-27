using UnityEngine;

[CreateAssetMenu(menuName = "Game/MiniBossConfigSet", fileName = "MiniBossConfigSet")]
public class MiniBossConfigSet : ScriptableObject
{
    [Header("MiniBoss that may appear in a wave sequence")]
    public MiniBossConfig[] miniBosses;
}
