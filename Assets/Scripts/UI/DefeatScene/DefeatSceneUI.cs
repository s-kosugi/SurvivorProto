using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DefeatSceneUI : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Light/Dark Levels & EXP")]
    [SerializeField] private TextMeshProUGUI lightLevelText;
    [SerializeField] private TextMeshProUGUI lightExpText;
    [SerializeField] private TextMeshProUGUI darkLevelText;
    [SerializeField] private TextMeshProUGUI darkExpText;
    [SerializeField] private TextMeshProUGUI killedByText;

    [Header("Buttons")]
    [SerializeField] private Button retryButton;
    [SerializeField] private Button titleButton;

    private void Start()
    {
        // --- SessionData を UI に反映 ---
        scoreText.text      = $"スコア: {SessionData.Score}";
        //lightLevelText.text = $"光レベル: {SessionData.LightLevel}";
        //lightExpText.text   = $"光EXP: {SessionData.LightExp}";
        //darkLevelText.text  = $"闇レベル: {SessionData.DarkLevel}";
        //darkExpText.text    = $"闇EXP: {SessionData.DarkExp}";

        switch (SessionData.KilledBy)
    {
        case EnemyID.Slime_Blue:
            killedByText.text = "スライム(青)にやられた…";
            break;
        case EnemyID.Slime_Yellow:
            killedByText.text = "スライム(黄)にやられた…";
            break;
        case EnemyID.Golem:
            killedByText.text = "ゴーレムに捕らわれた…";
            break;
        case EnemyID.Roper:
            killedByText.text = "ローパーに絡め取られた…";
            break;
        case EnemyID.Wraith:
            killedByText.text = "レイスに敗北…";
            break;
        case EnemyID.MeleeBoss:
            killedByText.text = "近距離ボスに敗北…";
            break;
        default:
            killedByText.text = "???";
            break;
    }


        retryButton.onClick.AddListener(() => SceneManager.LoadScene("GameMainScene"));
        titleButton.onClick.AddListener(() => SceneManager.LoadScene("TitleScene"));
    }
}