using UnityEngine;

public class DestroyOffscreen : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        
    }
    private void Update()
    {
        // Convert the asteroid's position from world space to viewport space.
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);

        // Check if the asteroid is outside the viewport.
        // Note: viewport coordinates are from 0 to 1, where (0,0) is the bottom-left and (1,1) is the top-right of the screen.
        if (viewportPosition.x < 0 || viewportPosition.x > 1 || viewportPosition.y < 0 || viewportPosition.y > 1)
        {
            Destroy(gameObject); // Destroy the asteroid
        }
    }
}
