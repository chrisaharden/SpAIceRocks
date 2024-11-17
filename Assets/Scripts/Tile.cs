using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x;
    public int y;
    public TileType type;
    public bool isLocked = false;
    public int coinValue = 0;
    public int purchasePrice = 0;

    public enum TileType
    {
        Type_00,
        Type_01,
        Type_02,
        Type_03,
        Type_04,
        Type_05,
        Type_06,
        Type_07,
        Type_08,
        Type_09,
        TOOL_PLUS_CLEARER,
        TOOL_COLUMN_CLEARER,
        TOOL_ROW_CLEARER,
        NONE
    }

    public bool CanPurchase()
    {
        return purchasePrice > 0 && GameManager.Instance.coinsEarned >= purchasePrice;
    }

    public bool TryPurchase()
    {
        if (CanPurchase())
        {
            GameManager.Instance.coinsEarned -= purchasePrice;
            UIManager.Instance.UpdateCoinsEarned(GameManager.Instance.coinsEarned);
            isLocked = false;
            return true;
        }
        return false;
    }
}
