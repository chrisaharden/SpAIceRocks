// 11/13/2024 AI-Tag
// This was created with assistance from Muse, a Unity Artificial Intelligence product
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public float moveDistance = 1.0f; // Distance to move the ship
    public float moveSpeed = 1.0f; // Speed of the movement

    public void MoveShipRight()
    {
        Vector3 targetPosition = transform.position + Vector3.right * moveDistance;
        StartCoroutine(MoveToPosition(targetPosition));
    }

    private System.Collections.IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
