using UnityEngine;

[CreateAssetMenu(fileName = "PlayerGrowthConfig", menuName = "Game/Player Growth Config")]
public class PlayerGrowthConfig : ScriptableObject
{
    [Header("Base HP")]
    public int baseHP = 4;

    [Header("Light Form HP Growth Levels")]
    public int[] lightHpUpLevels = new int[] { 3, 7 };

    [Header("Dark Form HP Growth Levels")]
    public int[] darkHpUpLevels = new int[] { 2, 4, 6, 8 };

    [Header("HP Increase Amount Per Trigger")]
    public int hpIncreaseAmount = 1;
}
