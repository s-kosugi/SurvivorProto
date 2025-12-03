using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources (assign in Inspector)")]
    [SerializeField] private AudioSource seSource;
    [SerializeField] private AudioSource bgmSource;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;
    public string seVolumeParameter = "SEVolume";
    public string bgmVolumeParameter = "BGMVolume";

    // ============================================================
    // SE 管理
    // ============================================================

    [System.Serializable]
    public class SEData
    {
        public string key;
        public AudioClip clip;
        public float baseVolume = 1f;
        public int maxSimultaneous = 1;
    }

    [Header("SE List")]
    public List<SEData> seList = new List<SEData>();
    private Dictionary<string, AudioClip> seDict;
    private Dictionary<string, float> seVolumeDict;


    // 同一フレーム再生制限
    private Dictionary<string, int> framePlayCount = new Dictionary<string, int>();
    private int lastFrame = -1;
    private Dictionary<string, int> seMaxSimultaneousDict;

    // ============================================================
    // BGM 管理
    // ============================================================

    [System.Serializable]
    public class BGMData
    {
        public string key;
        public AudioClip clip;
        public float baseVolume = 1f;   // 個別音量
    }

    [Header("BGM List")]
    public List<BGMData> bgmList = new List<BGMData>();

    private Dictionary<string, AudioClip> bgmDict;
    private Dictionary<string, float> bgmVolumeDict;

    // ============================================================
    // Unity Events
    // ============================================================

    private void Awake()
    {
        if (Instance != null)
        {
            // すでにSoundManagerが存在する → 今生成されたやつを破棄
            Destroy(gameObject);
            return;
        }

        // 初回だけ登録＆永続化
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // SEDictionary
        seDict = new Dictionary<string, AudioClip>(seList.Count);
        seVolumeDict = new Dictionary<string, float>(seList.Count);
        seMaxSimultaneousDict = new Dictionary<string, int>(seList.Count);
        foreach (var se in seList)
        {
            if (!string.IsNullOrEmpty(se.key) && se.clip != null)
            {
                seDict[se.key] = se.clip;
                seVolumeDict[se.key] = se.baseVolume;
                seMaxSimultaneousDict[se.key] = Mathf.Max(1, se.maxSimultaneous);
            }
        }

        // BGMDictionary
        bgmDict = new Dictionary<string, AudioClip>(bgmList.Count);
        bgmVolumeDict = new Dictionary<string, float>(bgmList.Count);

        foreach (var bgm in bgmList)
        {
            if (!string.IsNullOrEmpty(bgm.key) && bgm.clip != null)
            {
                bgmDict[bgm.key] = bgm.clip;
                bgmVolumeDict[bgm.key] = bgm.baseVolume;
            }
        }

        if (bgmSource != null)
            bgmSource.loop = true;
    }

    // ============================================================
    // SE 再生
    // ============================================================
    public void PlaySE(string key, float volume = 1f)
    {
        if (!seDict.TryGetValue(key, out var clip)) return;

        int currentFrame = Time.frameCount;

        if (currentFrame != lastFrame)
        {
            framePlayCount.Clear();
            lastFrame = currentFrame;
        }

        if (!framePlayCount.ContainsKey(key))
            framePlayCount[key] = 0;

        int limit = seMaxSimultaneousDict.TryGetValue(key, out var max) ? max : 1;
        if (framePlayCount[key] >= limit)
            return;

        float baseV = seVolumeDict.TryGetValue(key, out var v) ? v : 1f;

        seSource.PlayOneShot(clip, baseV * volume);
        framePlayCount[key]++;
    }


    // ============================================================
    // BGM 再生（キー指定）＋個別音量反映
    // ============================================================

    public void PlayBGM(string key, float fadeTime = 1f)
    {
        if (!bgmDict.TryGetValue(key, out var clip))
        {
            Debug.LogWarning($"[SoundManager] BGM not found: {key}");
            return;
        }

        float baseVolume = bgmVolumeDict.TryGetValue(key, out var v) ? v : 1f;

        StartCoroutine(FadeInBGM(clip, baseVolume, fadeTime));
    }

    private IEnumerator FadeInBGM(AudioClip newClip, float baseVolume, float duration)
    {
        if (bgmSource.clip == newClip && bgmSource.isPlaying)
            yield break;

        audioMixer.GetFloat(bgmVolumeParameter, out float db);
        float startVolume = Mathf.Pow(10, db / 20f);

        // フェードアウト
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float v = Mathf.Lerp(startVolume, 0f, t / duration);
            audioMixer.SetFloat(bgmVolumeParameter, LinearToDB(v));
            yield return null;
        }

        bgmSource.clip = newClip;
        bgmSource.volume = baseVolume;   // 個別音量を設定
        bgmSource.Play();

        // フェードイン
        t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float v = Mathf.Lerp(0f, baseVolume, t / duration);
            audioMixer.SetFloat(bgmVolumeParameter, LinearToDB(v));
            yield return null;
        }
    }

    public void StopBGM(float fadeTime = 1f)
    {
        StartCoroutine(FadeOutBGM(fadeTime));
    }

    private IEnumerator FadeOutBGM(float duration)
    {
        audioMixer.GetFloat(bgmVolumeParameter, out float db);
        float startVolume = Mathf.Pow(10, db / 20f);

        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float v = Mathf.Lerp(startVolume, 0f, t / duration);
            audioMixer.SetFloat(bgmVolumeParameter, LinearToDB(v));
            yield return null;
        }

        bgmSource.Stop();
    }

    // ============================================================
    // Mixer ヘルパー
    // ============================================================

    private float LinearToDB(float linear)
    {
        if (linear <= 0.0001f)
            return -80f;
        return Mathf.Log10(linear) * 20f;
    }

    public void SetSEVolume(float volume)
    {
        float db = Mathf.Lerp(-80f, 0f, volume);
        audioMixer.SetFloat(seVolumeParameter, db);
    }

    public float GetSEVolume()
    {
        if (audioMixer.GetFloat(seVolumeParameter, out float db))
            return Mathf.InverseLerp(-80f, 0f, db);
        return 1f;
    }

    public void SetBGMVolume(float volume)
    {
        float db = Mathf.Lerp(-80f, 0f, volume);
        audioMixer.SetFloat(bgmVolumeParameter, db);
    }

    public float GetBGMVolume()
    {
        if (audioMixer.GetFloat(bgmVolumeParameter, out float db))
            return Mathf.InverseLerp(-80f, 0f, db);
        return 1f;
    }
}
