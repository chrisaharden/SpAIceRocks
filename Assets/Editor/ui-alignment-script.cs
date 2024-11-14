// 11/14/2024 AI-Tag
// This was created with assistance from Muse, a Unity Artificial Intelligence product

using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;

public class UIAlignmentTool : EditorWindow
{
    [MenuItem("Tools/Align Horizontally Center")]
    private static void InitHorizontal()
    {
        AlignSelectedUIElementsHorizontally();
    }

    private static void AlignSelectedUIElementsHorizontally()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        
        if (selectedObjects.Length <= 1)
        {
            EditorUtility.DisplayDialog("UI Alignment Tool", 
                "Please select at least two UI elements to align.", "OK");
            return;
        }

        // Calculate the Y position of the first object
        float centerY = selectedObjects[0].GetComponent<RectTransform>().position.y;

        // Record the undo operation
        Undo.RecordObjects(selectedObjects.Select(obj => obj.GetComponent<RectTransform>())
            .Where(rt => rt != null).ToArray(), "Align UI Elements Horizontally");

        // Align all selected objects to the Y position of the first object
        foreach (GameObject obj in selectedObjects)
        {
            RectTransform rectTransform = obj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                Vector3 position = rectTransform.position;
                position.y = centerY;
                rectTransform.position = position;
                
                // Ensure the pivot point is centered horizontally
                rectTransform.pivot = new Vector2(rectTransform.pivot.x, 0.5f);
            }
        }
    }

    [MenuItem("Tools/Align Vertically Center")]
    private static void InitVertical()
    {
        AlignSelectedUIElementsVertically();
    }

    private static void AlignSelectedUIElementsVertically()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length <= 1)
        {
            EditorUtility.DisplayDialog("UI Alignment Tool", 
                "Please select at least two UI elements to align.", "OK");
            return;
        }

        // Calculate the X position of the first object
        float centerX = selectedObjects[0].GetComponent<RectTransform>().position.x;

        // Record the undo operation
        Undo.RecordObjects(selectedObjects.Select(obj => obj.GetComponent<RectTransform>())
            .Where(rt => rt != null).ToArray(), "Align UI Elements Vertically");

        // Align all selected objects to the X position of the first object
        foreach (GameObject obj in selectedObjects)
        {
            RectTransform rectTransform = obj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                Vector3 position = rectTransform.position;
                position.x = centerX;
                rectTransform.position = position;
                
                // Ensure the pivot point is centered vertically
                rectTransform.pivot = new Vector2(0.5f, rectTransform.pivot.y);
            }
        }
    }
}