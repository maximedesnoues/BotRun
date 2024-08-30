using UnityEngine;

public class CameraController : MonoBehaviour
{
    // References to the player's transform, camera offset, and smooth speed variables
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smoothSpeed;
    [SerializeField] private float rotationSpeed;

    // LateUpdate is called after all Update methods. Ensures camera follows player after all movements are done.
    private void LateUpdate()
    {
        FollowPlayer();
    }

    // Method to follow the player with a smooth transition and rotation
    private void FollowPlayer()
    {
        // Calculate desired camera position based on player position and offset
        Vector3 desiredPosition = playerTransform.position + playerTransform.TransformDirection(offset);

        // Smoothly interpolate between current and desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Rotate the camera to always look at the player
        Quaternion targetRotation = Quaternion.LookRotation(playerTransform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}