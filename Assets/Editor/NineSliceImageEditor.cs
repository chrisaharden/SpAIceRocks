// Editor/NineSliceImageEditor.cs (put this in an Editor folder)
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NineSliceImage))]
public class NineSliceImageEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        NineSliceImage nineSlice = (NineSliceImage)target;
        if (GUILayout.Button("Apply 9-Slice"))
        {
            nineSlice.SendMessage("SetupNineSlice");
        }
    }
}