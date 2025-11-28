using UnityEngine;

[CreateAssetMenu(fileName = "PlayerGrowthConfig", menuName = "Game/Player Growth Config")]
public class PlayerGrowthConfig : ScriptableObject
{
    // ============================
    // ■ HP 成長設定
    // ============================

    [Header("Base HP")]
    [Tooltip("HPの初期値（Light/Dark レベル 1 のときのHP）")]
    public int baseHP = 4;

    [Header("Light HP Growth Levels")]
    [Tooltip("Lightレベルがこの値以上になると HP が hpIncreaseAmount 分増える")]
    public int[] lightHpUpLevels = new int[] { 3, 7 };

    [Header("Dark HP Growth Levels")]
    [Tooltip("Darkレベルがこの値以上になると HP が hpIncreaseAmount 分増える")]
    public int[] darkHpUpLevels = new int[] { 2, 4, 6, 8 };

    [Header("HP Increase Amount per Trigger")]
    public int hpIncreaseAmount = 1;

    // ============================
    // ■ Light（射撃）正面ショット成長設定
    // ============================

    [System.Serializable]
    public class LightFrontShotGrowth
    {
        [Tooltip("この Light レベル以上で適用")]
        public int level;

        [Tooltip("扇形に撃つ弾数")]
        public int frontShotCount;

        [Tooltip("前方扇形の角度（例：45 → ±22.5°の範囲）")]
        public float frontAngle;
    }
    [Header("Light Front Shot Growth")]
    public LightFrontShotGrowth[] lightFrontShotGrowths;

    // ============================
    // ■ Light（射撃）N-Way成長設定
    // ============================

    [System.Serializable]
    public class LightNWayGrowth
    {
        [Tooltip("この Light レベル以上で適用")]
        public int level;

        [Tooltip("N-Way の弾数 (2,3,5,7 etc)")]
        public int nWayCount;
    }

    [Header("Light N-Way Shot Growth")]
    public LightNWayGrowth[] lightNWayGrowths;

    // ============================
    // ■ 近接攻撃（Dark）コンボ火力成長設定
    // ============================

    [System.Serializable]
    public class DarkMeleeGrowth
    {
        [Tooltip("この Dark レベル以上で適用")]
        public int level;

        [Tooltip("初段（ComboIndex 0）の追加ダメージ")]
        public int firstBonus;

        [Tooltip("2段目（ComboIndex 1）の追加ダメージ")]
        public int secondBonus;

        [Tooltip("3段目（ComboIndex 2）の追加ダメージ")]
        public int thirdBonus;
    }

    [Header("Dark Combo Damage Growth")]
    [Tooltip("Darkレベルに応じた近接コンボの火力強化設定")]
    public DarkMeleeGrowth[] darkComboGrowths;

    // ============================
    // ■ 近接攻撃（Dark）最大コンボ数成長設定
    // ============================

    [System.Serializable]
    public class DarkComboCountGrowth
    {
        [Tooltip("このレベル以上になるとコンボ数が maxCombo に更新される")]
        public int level;

        [Tooltip("最大コンボ数")]
        public int maxCombo;
    }

    [Header("Dark Combo Count Growth")]
    [Tooltip("Darkレベルに応じたコンボ段数の増加")]
    public DarkComboCountGrowth[] darkComboCountGrowths;
}
