using UnityEngine;

[System.Serializable]
public class TileConfig
{
    public TileType tileType;
    public bool isLocked = false;
    public int coinValue = 0;
    public int purchasePrice = 0;
    public int value = 0; 
    public string info = "";
    public int PlanetNumber = 0; 
    public GameObject tilePrefab;

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
}