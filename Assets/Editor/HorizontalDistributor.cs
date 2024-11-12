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

        // Verify all objects have RectTransform components
        foreach (GameObject obj in selectedObjects)
        {
            if (obj.GetComponent<RectTransform>() == null)
            {
                Debug.LogError($"Object '{obj.name}' doesn't have a RectTransform component. All objects must be UI elements.");
                return;
            }
        }

        // Find the parent Canvas width
        Canvas parentCanvas = selectedObjects[0].GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            Debug.LogError("Selected objects must be children of a Canvas.");
            return;
        }
        
        RectTransform canvasRect = parentCanvas.GetComponent<RectTransform>();
        float availableWidth = canvasRect.rect.width;

        // Calculate the total width needed
        float totalWidth = 0;
        foreach (GameObject obj in selectedObjects)
        {
            RectTransform rect = obj.GetComponent<RectTransform>();
            totalWidth += rect.rect.width * rect.localScale.x;
        }

        // Calculate the space between objects
        float spaceBetween = (availableWidth - totalWidth) / (selectedObjects.Length - 1);
        
        // Validate spacing
        if (spaceBetween < 0)
        {
            Debug.LogWarning("Objects may overlap - not enough space in canvas for selected distribution.");
        }

        // Calculate the starting position (from left edge of canvas)
        float currentX = -availableWidth / 2 + selectedObjects[0].GetComponent<RectTransform>().rect.width * selectedObjects[0].GetComponent<RectTransform>().localScale.x / 2;

        // Distribute the objects
        foreach (GameObject obj in selectedObjects)
        {
            RectTransform rect = obj.GetComponent<RectTransform>();
            
            // Set the anchor to middle-center for consistent positioning
            rect.anchorMin = new Vector2(0.5f, rect.anchorMin.y);
            rect.anchorMax = new Vector2(0.5f, rect.anchorMax.y);
            
            // Update position
            Vector3 newPosition = obj.transform.localPosition;
            newPosition.x = currentX;
            obj.transform.localPosition = newPosition;

            // Move to next position
            currentX += (rect.rect.width * rect.localScale.x) + spaceBetween;
        }
    }
}