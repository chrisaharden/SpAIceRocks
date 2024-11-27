// 11/27/2024 AI-Tag
// This was created with assistance from Muse, a Unity Artificial Intelligence product

using UnityEngine;
using UnityEditor;
using TMPro;

public class SetFontTool : Editor
{
    [MenuItem("Tools/Change All Fonts to Anton SDF")]
    public static void SetFont()
    {
        Undo.RecordObjects(Resources.FindObjectsOfTypeAll<TMP_FontAsset>(), "Set Font");
        // Get all Text Mesh Pro components in the scene
        var texts = Resources.FindObjectsOfTypeAll<TMP_Text>();
        foreach (TMP_Text text in texts)
        {
            TMP_FontAsset arialFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/Anton SDF");
            if (arialFont != null)
                text.font = arialFont;
            else
                Debug.LogWarning("Anton SDF font not found in project.");
            Debug.Log("Font of " + text.gameObject + " set to Anton SDF.");
        }
    }
}