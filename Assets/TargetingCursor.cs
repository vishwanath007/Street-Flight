using UnityEngine;
using UnityEngine.UI;

public class TargetingCursor : MonoBehaviour
{
    public Image cursorImage; // Assign this in the Inspector

    void Update()
    {
        // Toggle targeting mode with the Ctrl key
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            cursorImage.enabled = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            cursorImage.enabled = false;
        }

        // Move the cursor image to follow the mouse cursor
        if (cursorImage.enabled)
        {
            cursorImage.transform.position = Input.mousePosition;
        }
    }
}
