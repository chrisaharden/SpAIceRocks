using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x;
    public int y;
    public TileType type;

    public enum TileType
    {
        Yellow,
        Blue,
        Green,
        Red,
        Purple
    }
}
