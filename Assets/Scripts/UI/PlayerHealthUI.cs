using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image hpBarFill;

    void Start()
    {
        if (playerHealth != null)
        {
            playerHealth.HealthChanged += UpdateHPBar;
            UpdateHPBar(playerHealth.CurrentHP, playerHealth.MaxHP); // 初期値反映
        }
    }

    void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.HealthChanged -= UpdateHPBar;
        }
    }

    void UpdateHPBar(int current, int max)
    {
        if (hpBarFill == null) return;
        hpBarFill.fillAmount = (float)current / max;
    }
}
