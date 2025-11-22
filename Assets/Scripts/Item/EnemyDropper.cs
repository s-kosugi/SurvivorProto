using UnityEngine;

public class EnemyDropper : MonoBehaviour
{
    [Header("Drop Settings")]
    [Range(0f, 1f)]
    public float dropChance = 0.2f; // 20%でアイテム1個をDROP

    public DropTable dropTable;

    /// <summary>
    /// 敵が死んだ時に呼ぶ
    /// </summary>
    public void ExecuteDrop(Vector3 position)
    {
        if (Random.value > dropChance)
            return;

        if (dropTable == null)
            return;

        var prefab = dropTable.GetRandomItem();
        if (prefab != null)
        {
            Instantiate(prefab, position, Quaternion.identity);
        }
    }
}
