using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera;
    public Camera missileCamera;
    public Canvas missileCanvas; // The canvas or UI elements for the missile view

    // Start with the main camera active
    void Start()
    {
        mainCamera.enabled = true;
        missileCamera.enabled = false;
        missileCanvas.enabled = false; // Ensure the missile-specific UI is hidden initially
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(1)) // Right mouse button pressed
        {
            mainCamera.enabled = !mainCamera.enabled;
            missileCamera.enabled = !missileCamera.enabled;
            missileCanvas.enabled = missileCamera.enabled; // Show or hide missile UI accordingly
        }
        if (Input.GetMouseButtonUp(1)) // Right mouse button released
        {
            mainCamera.enabled = !mainCamera.enabled;
            missileCamera.enabled = !missileCamera.enabled;
            missileCanvas.enabled = missileCamera.enabled; // Show or hide missile UI accordingly
        }
    }
}
