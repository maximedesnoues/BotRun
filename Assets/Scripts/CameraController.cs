using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smoothSpeed;
    [SerializeField] private float rotationSpeed;

    private void LateUpdate()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        // Position de la caméra
        Vector3 desiredPosition = playerTransform.position + playerTransform.TransformDirection(offset);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Rotation de la caméra
        Quaternion targetRotation = Quaternion.LookRotation(playerTransform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}