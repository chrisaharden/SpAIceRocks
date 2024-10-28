using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x;
    public int y;
    public TileType type;

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
        Robot
    }
}
