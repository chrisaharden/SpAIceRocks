using UnityEditor;
using UnityEngine;
using System.Linq;

public class HorizontalDistributor : Editor
{
    [MenuItem("Tools/Horizontal Distributor")]
    public static void DistributeObjects()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        if (selectedObjects.Length <= 1)
        {
            Debug.LogError("Select at least two objects to distribute.");
            return;
        }

        // Find leftmost and rightmost objects based on anchoredPosition
        RectTransform[] rectTransforms = selectedObjects.Select(obj => obj.GetComponent<RectTransform>()).ToArray();
        
        if (rectTransforms.Any(rt => rt == null))
        {
            Debug.LogError("All selected objects must have RectTransform components.");
            return;
        }

        // Find the leftmost and rightmost positions based on anchoredPosition.x
        float leftX = rectTransforms.Min(rt => rt.anchoredPosition.x);
        float rightX = rectTransforms.Max(rt => rt.anchoredPosition.x);

        // Calculate total distance and step size
        float totalDistance = rightX - leftX;
        float step = totalDistance / (selectedObjects.Length - 1);

        // Sort objects by their current X position
        var sortedObjects = selectedObjects
            .OrderBy(obj => obj.GetComponent<RectTransform>().anchoredPosition.x)
            .ToArray();

        // Distribute objects
        for (int i = 0; i < sortedObjects.Length; i++)
        {
            RectTransform rectTransform = sortedObjects[i].GetComponent<RectTransform>();
            Vector2 newPos = rectTransform.anchoredPosition;
            newPos.x = leftX + (step * i);
            rectTransform.anchoredPosition = newPos;
        }

        // Ensure changes are saved
        EditorUtility.SetDirty(selectedObjects[0].transform.parent);
    }
}