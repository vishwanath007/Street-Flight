using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpaceshipController : MonoBehaviour
{
    public float movementSpeed = 10f;
    public float boostMultiplier = 2f; // Speed multiplier when boosting
    public float tiltAmount = 15f;
    public float smoothTilt = 2f;
    public float yawSpeed = 100f;
    public float pitchSpeed = 100f;
    public float maxTilt = 30f;
    public float forwardBackwardTiltFactor = 2f;
    public float gravityEffect = 9.81f; // Gravity effect on the spaceship

    private Rigidbody rb;
    public float health = 100f;

    public ParticleSystem fireEffect;

    private bool isSlowMotionActive = false;
    private float slowMotionDuration = 5f;
    private float slowMotionTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        // Use this to enable gravity effect on the spaceship.
        rb.useGravity = true; // We'll apply custom gravity for more control.
    }

    void Update()
    {
        if (health == 100f && fireEffect.isPlaying)
        {
            fireEffect.Stop();
        }
        HandleSlowMotion();
    }

    void HandleSlowMotion()
    {
        // Check if 'T' is pressed and slow motion is not already active
        if (Input.GetKeyDown(KeyCode.T) && !isSlowMotionActive)
        {
            StartCoroutine(ActivateSlowMotion());
        }

        // If slow motion is active, count down the timer
        if (isSlowMotionActive)
        {
            slowMotionTimer -= Time.unscaledDeltaTime; // Use unscaledDeltaTime so the countdown isn't affected by timeScale
            if (slowMotionTimer <= 0)
            {
                // When timer runs out, deactivate slow motion
                Time.timeScale = 1f;
                isSlowMotionActive = false;
            }
        }
    }

    System.Collections.IEnumerator ActivateSlowMotion()
    {
        Time.timeScale = 0.1f; // Set to half speed; adjust as needed
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // Adjust fixedDeltaTime to keep physics interactions smooth
        isSlowMotionActive = true;
        slowMotionTimer = slowMotionDuration;

        // Wait for the duration of the slow motion effect
        yield return new WaitForSecondsRealtime(slowMotionDuration);

        // Ensure these lines are reached only if slow motion wasn't prematurely deactivated
        if (isSlowMotionActive)
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f; // Reset fixedDeltaTime
            isSlowMotionActive = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        float damage = 10f;
        health -= damage;

        if (health < 50)
        {
            if (!fireEffect.isPlaying)
            {
                fireEffect.Play();
            }
        }

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void HandleMobileControls()
    {
        // Mobile controls implementation remains unchanged
    }

    void FixedUpdate()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            HandleMobileControls();
        }
        else
        {
            HandlePCControls();
        }

        // Apply a constant downward force to simulate gravity.
        //rb.AddForce(Vector3.down * gravityEffect, ForceMode.Acceleration);

        // Gradually return to neutral when there's no input
        ResetRotationToNeutral();
    }

    void HandlePCControls()
    {
        bool isBoosting = Input.GetKey(KeyCode.Space);
  
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Movement
        Vector3 movement = transform.right * moveHorizontal + transform.forward * moveVertical;
        rb.velocity = movement * movementSpeed;

        // Check if boosting
        if (isBoosting)
        {
            // Apply an upward force instead of directly changing the velocity.
            // The ForceMode.Acceleration ignores the mass of the rigidbody, leading to a consistent acceleration.
            rb.AddForce(Vector3.up * boostMultiplier, ForceMode.Acceleration);
        }
        // Yaw control with mouse or Q and E keys
        float yawInput = Input.GetAxis("Mouse X") + (Input.GetKey(KeyCode.E) ? 1 : 0) - (Input.GetKey(KeyCode.Q) ? 1 : 0);
        float yaw = yawInput * yawSpeed * Time.deltaTime;

        float pitch = -Input.GetAxis("Mouse Y") * pitchSpeed * Time.deltaTime;

        Quaternion additionalRotation = Quaternion.Euler(pitch, yaw, 0f);

        float pitchAdjustment = moveVertical * forwardBackwardTiltFactor;
        float targetRoll = Mathf.Clamp(moveHorizontal * -tiltAmount, -maxTilt, maxTilt);
        float targetPitch = Mathf.Clamp((moveVertical * -tiltAmount) + pitchAdjustment, -maxTilt, maxTilt);

        // Applying the combined rotation including the yaw from Q and E keys
        Quaternion targetRotation = Quaternion.Euler(0, rb.rotation.eulerAngles.y + yaw, targetRoll);
        rb.rotation = Quaternion.Lerp(rb.rotation, targetRotation, Time.deltaTime * smoothTilt) * additionalRotation;
    }





    private void ResetRotationToNeutral()
    {
        if (Mathf.Abs(Input.GetAxis("Horizontal")) < 0.1f && Mathf.Abs(Input.GetAxis("Vertical")) < 0.1f)
        {
            Quaternion neutralRotation = Quaternion.Euler(0f, rb.rotation.eulerAngles.y, 0f);
            rb.rotation = Quaternion.Lerp(rb.rotation, neutralRotation, Time.deltaTime * smoothTilt);
        }
    }
}