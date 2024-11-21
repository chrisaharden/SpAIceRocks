using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileConfig config;
    public int x;
    public int y;

    public bool CanPurchase()
    {
        return config.purchasePrice > 0 && GameManager.Instance.coinsEarned >= config.purchasePrice;
    }

    public bool TryPurchase()
    {
        if (CanPurchase())
        {
            GameManager.Instance.coinsEarned -= config.purchasePrice;
            UIManager.Instance.UpdateCoinsEarned(GameManager.Instance.coinsEarned);
            config.isLocked = false;
            return true;
        }
        return false;
    }
}
