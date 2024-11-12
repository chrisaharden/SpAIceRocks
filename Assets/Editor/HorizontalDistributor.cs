using UnityEditor;
using UnityEngine;

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

        // Find the parent Canvas and its RectTransform
        Canvas parentCanvas = selectedObjects[0].GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            Debug.LogError("Selected objects must be children of a Canvas.");
            return;
        }

        RectTransform canvasRect = parentCanvas.GetComponent<RectTransform>();
        
        // Get the usable width (let's use 80% of the canvas width to keep buttons within visible area)
        float safeWidth = canvasRect.rect.width * 0.8f;
        
        // Calculate total width of all objects plus desired padding
        float padding = 20f; // Padding between buttons
        float totalWidth = (selectedObjects.Length - 1) * padding;
        
        foreach (GameObject obj in selectedObjects)
        {
            RectTransform rect = obj.GetComponent<RectTransform>();
            if (rect == null)
            {
                Debug.LogError($"Object '{obj.name}' doesn't have a RectTransform component.");
                return;
            }
            totalWidth += rect.rect.width;
        }

        // Calculate starting X position (centered)
        float currentX = -totalWidth / 2;

        // Distribute objects
        foreach (GameObject obj in selectedObjects)
        {
            RectTransform rect = obj.GetComponent<RectTransform>();
            
            // Center the anchors
            rect.anchorMin = new Vector2(0.5f, rect.anchorMin.y);
            rect.anchorMax = new Vector2(0.5f, rect.anchorMax.y);
            rect.pivot = new Vector2(0.5f, rect.pivot.y);

            // Set position
            Vector2 newPosition = rect.anchoredPosition;
            newPosition.x = currentX;
            rect.anchoredPosition = newPosition;

            // Move to next position
            currentX += rect.rect.width + padding;
        }
    }
}