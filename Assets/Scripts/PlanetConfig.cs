using System;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class PlanetConfig
{
    public bool isLocked;
    public int purchasePrice;
    public string info = "";
    public int planetNumber;
    public float ShipPosX;
    public bool ShipFlyOrJump; // True for fly, false for jump
    public Sprite BackgroundImage; 
    public Sprite CharacterSprite;  
}
