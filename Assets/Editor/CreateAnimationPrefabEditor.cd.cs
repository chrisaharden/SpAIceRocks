using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEditor.Animations;

[CustomEditor(typeof(NineSliceImage))]
public class CreateAnimationPrefabEditor : Editor
{
    [MenuItem("Tools/Create Animation Prefab")]
    public static void CreateAnimationPrefab()
    {
        // Create a new text object
        GameObject textObject = new GameObject("TextMoveAndFade");
        TextMeshProUGUI textComponent = textObject.AddComponent<TextMeshProUGUI>();
        textComponent.text = "+00";

        // Create an animator component
        Animator animator = textObject.AddComponent<Animator>();
        AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPathWithClip("Assets/Animations/TextMoveAndFadeController.controller", new AnimationClip());
        animator.runtimeAnimatorController = animatorController;

        // Create a new animation clip
        AnimationClip animationClip = new AnimationClip();

        // Create a new animation curve for position
        AnimationCurve positionCurve = AnimationCurve.Linear(0, 0, 1, 20);
        animationClip.SetCurve("", typeof(RectTransform), "m_AnchorMin.y", positionCurve);

        // Create a new animation curve for color
        AnimationCurve colorCurve = AnimationCurve.Linear(0, 1, 1, 0);
        animationClip.SetCurve("", typeof(CanvasGroup), "m_Alpha", colorCurve);

        // Add the animation clip to the animator controller
        AssetDatabase.AddObjectToAsset(animationClip, "Assets/Animations/TextMoveAndFadeController.controller");
        animatorController.AddMotion(animationClip);

        // Save the animation clip as a prefab
        string prefabPath = "Assets/Animations/TextMoveAndFade.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(textObject, prefabPath);
        Debug.Log("TextMoveAndFade animation prefab created at " + prefabPath);

        // Clean up
        GameObject.DestroyImmediate(textObject);
    }
}