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

        // Find leftmost and rightmost objects
        GameObject leftmost = selectedObjects[0];
        GameObject rightmost = selectedObjects[0];
        float leftX = selectedObjects[0].transform.position.x;
        float rightX = selectedObjects[0].transform.position.x;

        foreach (GameObject obj in selectedObjects)
        {
            float objX = obj.transform.position.x;
            if (objX < leftX)
            {
                leftX = objX;
                leftmost = obj;
            }
            if (objX > rightX)
            {
                rightX = objX;
                rightmost = obj;
            }
        }

        // Calculate total distance and step size
        float totalDistance = rightX - leftX;
        float step = totalDistance / (selectedObjects.Length - 1);

        // Sort objects by their current X position
        GameObject[] sortedObjects = selectedObjects
            .OrderBy(obj => obj.transform.position.x)
            .ToArray();

        // Distribute objects
        for (int i = 0; i < sortedObjects.Length; i++)
        {
            GameObject obj = sortedObjects[i];
            Vector3 newPos = obj.transform.position;
            newPos.x = leftX + (step * i);
            obj.transform.position = newPos;
        }
    }
}