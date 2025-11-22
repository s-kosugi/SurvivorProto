using UnityEngine;

[CreateAssetMenu(menuName = "Game/DropTable")]
public class DropTable : ScriptableObject
{
    public DropCandidate[] candidates;

    /// <summary>
    /// 候補から1つ重み付きで抽選
    /// </summary>
    public GameObject GetRandomItem()
    {
        if (candidates == null || candidates.Length == 0)
            return null;

        // 重みの合計
        float totalWeight = 0f;
        foreach (var c in candidates)
            totalWeight += c.weight;

        float r = Random.value * totalWeight;

        float sum = 0f;
        foreach (var c in candidates)
        {
            sum += c.weight;
            if (r <= sum)
                return c.itemPrefab;
        }

        return null;
    }
}
