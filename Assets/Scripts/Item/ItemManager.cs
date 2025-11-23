using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    private List<GameObject> items = new List<GameObject>();
    private Transform itemRoot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        itemRoot = this.transform;
    }

    public void Register(GameObject item)
    {
        if (!items.Contains(item))
            items.Add(item);

        // 生成位置に関係なくRoot配下へ移動
        item.transform.SetParent(itemRoot);
    }

    public void Unregister(GameObject item)
    {
        if (items.Contains(item))
            items.Remove(item);
    }

    public void ClearAll()
    {
        foreach (var item in items)
        {
            if (item != null)
                Destroy(item);
        }
        items.Clear();
    }
}
