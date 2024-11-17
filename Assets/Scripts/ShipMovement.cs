// 11/13/2024 AI-Tag
// This was created with assistance from Muse, a Unity Artificial Intelligence product
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public float moveSpeed = 1.0f; // Speed of the movement
    public float hoverHeight = 0.1f; // How high the ship floats up and down
    public float hoverSpeed = 1.0f; // Speed of the hovering motion
    private Vector3 startPosition; // Store the initial position
    private float hoverTime; // Track time for hover effect

    void Start()
    {
        startPosition = transform.position;
        hoverTime = 0f;
    }

    private void OnEnable()
    {
        // Initialize startPosition if needed
        startPosition = transform.position;
        // Start the HoverEffect coroutine
        StartCoroutine(HoverEffect());
    }

    private void OnDisable()
    {
        // Stop the HoverEffect coroutine when the GameObject is disabled
        StopCoroutine(HoverEffect());
    }

    public void MoveShipToX(float targetX, bool ShipFlyOrJump) // True for fly, false for jump
    {
        Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);
        startPosition = targetPosition; 
        if(ShipFlyOrJump)
            StartCoroutine(MoveToPosition(targetPosition));
        else
            transform.position = targetPosition;
    }

    private System.Collections.IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(new Vector3(transform.position.x, startPosition.y, transform.position.z), targetPosition) > 0.01f)
        {
            Vector3 newPosition = Vector3.MoveTowards(
                new Vector3(transform.position.x, startPosition.y, transform.position.z),
                targetPosition,
                moveSpeed * Time.deltaTime
            );
            transform.position = new Vector3(newPosition.x, transform.position.y, newPosition.z);
            yield return null;
        }
    }

    private System.Collections.IEnumerator HoverEffect()
    {
        while (true)
        {
            hoverTime += Time.deltaTime * hoverSpeed;
            float yOffset = Mathf.Sin(hoverTime) * hoverHeight;
            transform.position = new Vector3(
                transform.position.x,
                startPosition.y + yOffset,
                transform.position.z
            );
            Debug.Log("Hovering: " + transform.position.y); // Debug log to check if coroutine is running
            yield return null;
        }
    }
}
