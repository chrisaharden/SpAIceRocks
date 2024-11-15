// 11/15/2024 AI-Tag
// This was created with assistance from Muse, a Unity Artificial Intelligence product

using UnityEditor;
using UnityEngine;

public class Cheats : EditorWindow
{
    private static void Init()
    {
    }

    [MenuItem("Tools/Cheats/Cheap Planets")]
    private static void CheaPlanets()
    {
        SetPlanetPurchasePricesToCheap();
        GiveMeMoney();
    }

    [MenuItem("Tools/Cheats/Nearly Out Of Moves")]
    private static void NearlyOutOfMoves()
    {
        GameObject boardObject = GameObject.Find("Board");
        Board board = boardObject.GetComponent<Board>();
        if (board != null)
        {
            Undo.RecordObject(board, "Nearly Out Of Moves");
            board.movesRemaining = 1; 
        }
    }

    private static void SetPlanetPurchasePricesToCheap()
    {
        GameObject gameManagerObject = GameObject.Find("GameManager");
        GameManager gameManager = gameManagerObject.GetComponent<GameManager>();
        if (gameManager != null)
        {
            Undo.RecordObject(gameManager, "Cheap Planets");
            foreach (PlanetConfig planetConfig in gameManager.planetConfigs)
            {
                planetConfig.purchasePrice = 1;
            }
        }
    }

    private static void GiveMeMoney()
    {
        GameObject gameManagerObject = GameObject.Find("GameManager");
        GameManager gameManager = gameManagerObject.GetComponent<GameManager>();
        if (gameManager != null)
        {
            Undo.RecordObject(gameManager, "Give Me Money");
            gameManager.coinsEarned = 1000;
        }
    }
}