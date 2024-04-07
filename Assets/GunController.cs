using UnityEngine;

public class GunController : MonoBehaviour
{
    public Camera playerCamera; // Assign your main camera
    public float shootingRange = 100f;
    public GameObject laserBeamPrefab; // Assign a prefab for the laser effect
    private LineRenderer laserLineRenderer;
    public float laserWidth = 0.1f;
    public float laserMaxLength = 5f;

    void Start()
    {
        // Initialize the laser line renderer
        GameObject laserBeam = Instantiate(laserBeamPrefab);
        laserLineRenderer = laserBeam.GetComponent<LineRenderer>();
        Vector3[] initLaserPositions = new Vector3[2] { Vector3.zero, Vector3.zero };
        laserLineRenderer.SetPositions(initLaserPositions);
        laserLineRenderer.startWidth = laserWidth;
        laserLineRenderer.endWidth = laserWidth;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1")) // Default mouse left click
        {
            Shoot();
        }

        DrawLaser();
    }

    void Shoot()
    {
        RaycastHit hit;
        // Raycast from the camera to detect hits
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, shootingRange))
        {
            Debug.Log(hit.transform.name);
            // Add logic here to handle what happens when you hit an asteroid
            // For example, destroy the asteroid
            if (hit.collider.tag == "Asteroid")
            {
                Destroy(hit.collider.gameObject);
            }
        }
    }

    void DrawLaser()
    {
        // Adjust the laser to start from the gun and point forward
        if (laserLineRenderer != null)
        {
            laserLineRenderer.SetPosition(0, transform.position);
            laserLineRenderer.SetPosition(1, transform.position + transform.forward * laserMaxLength);
        }
    }
}
