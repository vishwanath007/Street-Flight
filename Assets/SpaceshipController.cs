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
    public FixedJoystick joyStick;
    private Rigidbody rb;
    public float health = 100f;

    public ParticleSystem fireEffect;
    public ParticleSystem MissileEffect;

    private bool isSlowMotionActive = false;
    private float slowMotionDuration = 5f;
    private float slowMotionTimer = 0f;


    public GameObject missilePrefab;
    public Transform launchPosition;
    public Transform currentTarget;
    private bool isTargetingMode = false;

    private AudioSource audioSource;
    public AudioClip launchSound; // Launch sound clip
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

        // Toggle targeting mode with the Ctrl key
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isTargetingMode = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isTargetingMode = false;
        }

        // Fire missile with right mouse button during targeting mode
        if (Input.GetMouseButtonDown(0))//isTargetingMode || Input.GetMouseButtonDown(1)) // 1 is the right mouse button
        {
            FireMissile();//FireMissileAtTarget();
        }
    }


    void FireMissileAtTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Assuming your missile script looks for a 'target' Transform
            currentTarget = hit.collider.transform;
            FireMissile();
        }
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
        bool isBoosting = Input.GetKey(KeyCode.Space);

        float moveHorizontal = joyStick.Horizontal;//Input.GetAxis("Horizontal");
        float moveVertical = joyStick.Vertical;

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


    public void FireMissile()
    {
        var explosion = Instantiate(MissileEffect, launchPosition.position, Quaternion.identity);
        explosion.Play();
        // Destroy the explosion effect after it has finished
        Destroy(explosion.gameObject, explosion.main.duration);

        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = launchSound;
        audioSource.volume = 1;
        audioSource.spatialBlend = 1; // Make it 3D sound
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.maxDistance = 500;
        audioSource.Play();


        GameObject missile = Instantiate(missilePrefab, launchPosition.position, launchPosition.rotation);
        Destroy(audioSource, launchSound.length);
        //missile.GetComponent<Missile>().target = currentTarget;

        //if (currentTarget != null)
        //{
        //    Vector3 targetDirection = (currentTarget.position - missile.transform.position).normalized;
        //    missile.transform.rotation = Quaternion.LookRotation(targetDirection);
        //}
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
