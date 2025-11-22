using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<ItemBase>(out var item))
        {
            Debug.Log("Trigger hit: " + other.name);
            // PlayerManager からプレイヤーを取得
            var player = PlayerManager.Instance.MainPlayer;

            if (player != null)
            {
                Debug.Log("Player Enable");
                item.Collect(player);
            }
        }
    }
}