// 11/15/2024 AI-Tag
// This was created with assistance from Muse, a Unity Artificial Intelligence product

using UnityEditor;
using UnityEngine;

public class SetPlanetPurchasePricesToCheap : EditorWindow
{
    [MenuItem("Tools/Cheats/Set Planet Prices To Cheap")]
    private static void Init()
    {
        SetPurchasePriceToAllPlanets();
    }

    private static void SetPurchasePriceToAllPlanets()
    {
        GameObject gameManagerObject = GameObject.Find("GameManager");
        GameManager gameManager = gameManagerObject.GetComponent<GameManager>();
        if (gameManager != null)
        {
            Undo.RecordObject(gameManager, "Set Purchase Price to All Planets");
            foreach (PlanetConfig planetConfig in gameManager.planetConfigs)
            {
                planetConfig.purchasePrice = 1;
            }
        }
    }
}