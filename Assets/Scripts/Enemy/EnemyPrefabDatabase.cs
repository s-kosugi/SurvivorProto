using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy/EnemyPrefabDatabase")]
public class EnemyPrefabDatabase : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public EnemyID id;
        public GameObject prefab;
    }

    public Entry[] entries;

    // 取得メソッド
    public GameObject GetPrefab(EnemyID id)
    {
        foreach (var e in entries)
        {
            if (e.id == id)
                return e.prefab;
        }

        Debug.LogError($"EnemyPrefabDatabase: {id} のPrefabが見つかりません");
        return null;
    }
}
