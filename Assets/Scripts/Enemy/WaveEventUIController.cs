using UnityEngine;
using TMPro;

public class WaveEventUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private float fadeDuration = 1.0f;

    public void ShowWaveStart()
    {
        ShowText("MINI BOSS APPEARS!");
    }

    public void ShowWaveClear()
    {
        ShowText("WAVE CLEAR! +HP");
    }

    private void ShowText(string message)
    {
        if (waveText == null) return;

        waveText.text = message;
        waveText.alpha = 1;
        StartCoroutine(FadeOut());
    }

    private System.Collections.IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            waveText.alpha = 1 - (t / fadeDuration);
            yield return null;
        }

        waveText.alpha = 0;
    }
    public void HideAll()
    {
        waveText?.gameObject.SetActive(false);
    }
}
