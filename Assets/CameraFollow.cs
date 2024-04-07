using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Assign your spaceship as the target in the inspector
    public Vector3 offset = new Vector3(0, 5, -10); // Adjust this offset for the desired camera position relative to the spaceship
    public float smoothSpeed = 0.125f; // Adjust for smoother camera movement
    public float rotationSmoothSpeed = 5f; // Adjust for smoother rotation matching

    private void Start()
    {
        offset = transform.position - target.position;
    }
    void LateUpdate()
    {
        // Position
        Vector3 desiredPosition = target.position + target.TransformDirection(offset);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Rotation
        Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothSpeed * Time.deltaTime);
    }
}
