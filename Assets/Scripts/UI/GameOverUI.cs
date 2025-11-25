using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Button restartButton;

    public System.Action OnRestart;

    private void Awake()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    public void Show(int score)
    {
        if (panel != null)
            panel.SetActive(true);

        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }

    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    public void BindRestart(System.Action action)
    {
        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(() => action?.Invoke());
    }
}
