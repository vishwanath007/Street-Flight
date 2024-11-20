using UnityEngine;

public class Missile : MonoBehaviour
{
    public Transform target;
    public float speed = 10f;
    public float turnSpeed = 50f;
    public ParticleSystem explosionEffect;

    public AudioClip explosionSound; // Explosion sound clip

    private Rigidbody rb;
    private bool hasCollided = false;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Destroy(gameObject, 3f);

    }

    void FixedUpdate()
    {
        rb.velocity = transform.forward * speed;
        /*
        if (target != null && !hasCollided)
        {
            Vector3 targetDirection = (target.position - transform.position).normalized;
            float step = turnSpeed * Time.fixedDeltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, step, 0.0f);
            rb.rotation = Quaternion.LookRotation(newDirection);
            
        }
        */
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!hasCollided)
        {
            hasCollided = true;
            PlayExplosionSound(); // Play explosion sound on collision
            if (explosionEffect != null)
            {
                var explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
                explosion.Play();
                Destroy(explosion.gameObject, explosion.main.duration);
            }
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }

    private void PlayExplosionSound()
    {
        // Create a temporary GameObject to play the explosion sound
        GameObject tempAudioSource = new GameObject("TempAudio");
        tempAudioSource.transform.position = transform.position; // Position it at the missile's position

        AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
        audioSource.clip = explosionSound;
        audioSource.volume = 1;
        audioSource.spatialBlend = 1; // Make it 3D sound
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.maxDistance = 500;
        audioSource.Play();

        Destroy(tempAudioSource, explosionSound.length); // Destroy the temp object after the clip has played
    }
}
