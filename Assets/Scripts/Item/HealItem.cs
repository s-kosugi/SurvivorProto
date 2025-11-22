using UnityEngine;

public class HealItem : ItemBase
{
    public int healAmount = 10;

    public override void Collect(PlayerCore player)
    {
        if (player != null && player.health != null)
        {
            player.health.RecoverHP(healAmount);
        }

        Destroy(gameObject);
    }
}
