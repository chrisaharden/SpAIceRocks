// 11/12/2024 AI-Tag
// This was created with assistance from Muse, a Unity Artificial Intelligence product

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

        // Calculate the total width needed
        float totalWidth = 0;
        foreach (GameObject obj in selectedObjects)
        {
            totalWidth += obj.GetComponent<RectTransform>().sizeDelta.x;
        }

        // Calculate the space between objects
        float spaceBetween = (SceneView.lastActiveSceneView.position.width - totalWidth) / (selectedObjects.Length - 1);

        // Calculate the starting position
        float startPosition = -totalWidth / 2 + spaceBetween / 2;

        // Distribute the objects
        foreach (GameObject obj in selectedObjects)
        {
            startPosition += obj.GetComponent<RectTransform>().sizeDelta.x / 2;
            obj.transform.localPosition = new Vector3(startPosition, obj.transform.localPosition.y, obj.transform.localPosition.z);
            startPosition += obj.GetComponent<RectTransform>().sizeDelta.x / 2 + spaceBetween;
        }
    }
}