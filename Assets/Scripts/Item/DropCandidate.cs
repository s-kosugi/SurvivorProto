using UnityEngine;

[System.Serializable]
public class DropCandidate
{
    public GameObject itemPrefab;

    [Tooltip("このアイテムが選ばれる確率の重み（例：Exp=80, 回復=5, レア=1）")]
    public float weight = 1f;
}
