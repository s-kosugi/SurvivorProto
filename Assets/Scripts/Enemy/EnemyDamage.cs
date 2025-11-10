using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private int contactDamage = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(contactDamage);
        }
    }
}
