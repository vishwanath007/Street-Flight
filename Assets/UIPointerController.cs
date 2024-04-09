using UnityEngine;
using UnityEngine.UI;

public class UIPointerController : MonoBehaviour
{
    public Image uiPointer; // Assign your UI Pointer in the Inspector
    public Camera playerCamera; // Assign the main camera
    public float maxRaycastDistance = 1000f; // Maximum distance for raycast

    void Update()
    {

        // Toggle crosshair visibility when the right mouse button is pressed
        if (Input.GetMouseButtonDown(1)) // Right mouse button pressed
        {
            ShowPointer(true); // Hide crosshair
        }
        if (Input.GetMouseButtonUp(1)) // Right mouse button released
        {
            ShowPointer(false); // Show crosshair
        }
    }

    public void ShowPointer(bool visible)
    {

        uiPointer.enabled = visible;
        // Convert mouse position to a ray
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRaycastDistance))
        {
            // If the ray hits something in the world, position the UI pointer over that point
            Vector3 pointerPosition = playerCamera.WorldToScreenPoint(hit.point);
            uiPointer.transform.position = pointerPosition;
        }
    }
 }
