public static class SessionData
{
    // --- プレイヤーデータ ---
    public static int LightLevel;
    public static int DarkLevel;

    public static int LightExp;
    public static int DarkExp;

    // --- 死亡時の情報（βで各敵ごほうびシーン用） ---
    public static EnemyID KilledBy;  
    // 例："Slime" "Reaper" など

    // --- スコア関連（今は使わないけど残しておくと便利） ---
    public static int Score;
    // public static int HighScore;   // 必要になった時にPlayerPrefsで保存すると良い

    // --- セッション開始前にクリアしたい時用のヘルパー ---
    public static void Clear()
    {
        LightLevel = 1;
        DarkLevel = 1;

        LightExp = 0;
        DarkExp = 0;

        KilledBy = EnemyID.Slime_Blue;
        // LastScore = 0;
    }
}
