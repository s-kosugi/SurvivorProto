using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerExpUI : MonoBehaviour
{
    [SerializeField] private PlayerExpCollector exp;

    [Header("Light")]
    [SerializeField] private TMP_Text lightLevelText;
    [SerializeField] private Image lightExpFill;

    [Header("Dark")]
    [SerializeField] private TMP_Text darkLevelText;
    [SerializeField] private Image darkExpFill;

    void Start()
    {
        if (exp != null)
        {
            exp.OnExpChanged += UpdateUI; // ← イベント追加した方が綺麗
            UpdateUI();
        }
    }

    private void OnDisable()
    {
        if (exp != null)
            exp.OnExpChanged -= UpdateUI;
    }

    private void UpdateUI()
    {
        int reqL = exp.GetRequiredExp(exp.lightLevel);
        int reqD = exp.GetRequiredExp(exp.darkLevel);

        lightLevelText.text = $"Light Lv {exp.lightLevel}";
        darkLevelText.text = $"Dark Lv {exp.darkLevel}";

        lightExpFill.fillAmount = (float)exp.lightExp / reqL;
        darkExpFill.fillAmount = (float)exp.darkExp / reqD;
    }
}
