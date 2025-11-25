using UnityEngine;
using TMPro;
using System.Collections;

public class WaveEventUIController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject miniBossStartPanel;
    [SerializeField] private GameObject waveClearPanel;
    [SerializeField] private TMP_Text miniBossStartText;
    [SerializeField] private TMP_Text waveClearText;

    [Header("Fade Settings")]
    [SerializeField] private float fadeInTime = 0.3f;
    [SerializeField] private float showTime = 1.0f;
    [SerializeField] private float fadeOutTime = 0.5f;

    private Coroutine currentRoutine;

    private void Start()
    {
        if (WaveEventManager.Instance != null)
        {
            WaveEventManager.Instance.OnMiniBossCleared += ShowWaveClear;
            WaveEventManager.Instance.OnMiniBossStart += ShowMiniBossStart;
        }

        ResetUI();
    }

    private void OnDisable()
    {
        if (WaveEventManager.Instance != null)
        {
            WaveEventManager.Instance.OnMiniBossCleared -= ShowWaveClear;
            WaveEventManager.Instance.OnMiniBossStart -= ShowMiniBossStart;
        }
    }

    // すべてのイベントUIを非表示
    public void ResetUI()
    {
        if (miniBossStartPanel) miniBossStartPanel.SetActive(false);
        if (waveClearPanel) waveClearPanel.SetActive(false);
    }

    public void ShowMiniBossStart()
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(FadeEvent(miniBossStartPanel, miniBossStartText, "MiniBoss Appeared!"));
    }

    public void ShowWaveClear()
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(FadeEvent(waveClearPanel, waveClearText, "Wave Clear!!"));
    }

    private IEnumerator FadeEvent(GameObject panel, TMP_Text text, string message)
    {
        // 他のパネルは消す
        ResetUI();

        // 表示メッセージ適用
        text.text = message;

        // Panel を有効化
        panel.SetActive(true);

        // CanvasGroup を確保
        CanvasGroup group = panel.GetComponent<CanvasGroup>();
        if (group == null)
            group = panel.AddComponent<CanvasGroup>();

        group.alpha = 0f;

        // fade-in
        float t = 0f;
        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Lerp(0f, 1f, t / fadeInTime);
            yield return null;
        }
        group.alpha = 1f;

        // 一定時間表示
        yield return new WaitForSeconds(showTime);

        // fade-out
        t = 0f;
        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Lerp(1f, 0f, t / fadeOutTime);
            yield return null;
        }

        panel.SetActive(false);
        group.alpha = 0f;
    }
}
