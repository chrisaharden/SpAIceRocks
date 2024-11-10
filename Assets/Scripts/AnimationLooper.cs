// 11/9/2024 AI-Tag
// This was created with assistance from Muse, a Unity Artificial Intelligence product

using System;
using UnityEditor;
using UnityEngine;

public class AnimationLooper : MonoBehaviour
{
    public Animator animator;
    public string animationName;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Trigger on space key press
        {
            AdvanceAnimationFrame();
        }
    }

    void AdvanceAnimationFrame()
    {
        if (animator == null) return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float normalizedTime = stateInfo.normalizedTime % 1; // Get the current time within the loop
        float newTime = normalizedTime + (1f / stateInfo.length); // Move to the next frame

        if (newTime >= 1f)
        {
            newTime = 0f; // Wrap around to the first frame
        }

        animator.Play(animationName, 0, newTime);
    }
}
