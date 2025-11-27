using UnityEngine;

public class GameBalanceLoader : MonoBehaviour
{
    public BalanceSet balance;

    public static BalanceSet Current;

    private void Awake()
    {
        // ゲーム開始時に選択中のBalanceセットを適用
        Current = balance;
    }
}
