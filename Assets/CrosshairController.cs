using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    private Image crosshairImage;

    void Start()
    {
        // Get the Image component from this GameObject
        crosshairImage = GetComponent<Image>();
    }

    void Update()
    {
        // Toggle crosshair visibility when the right mouse button is pressed
        if (Input.GetMouseButtonDown(1)) // Right mouse button pressed
        {
            ShowCrosshair(true); // Hide crosshair
        }
        if (Input.GetMouseButtonUp(1)) // Right mouse button released
        {
            ShowCrosshair(false); // Show crosshair
        }
    }

    // Method to set the crosshair visibility
    public void ShowCrosshair(bool visible)
    {
        crosshairImage.enabled = visible;
    }
}
