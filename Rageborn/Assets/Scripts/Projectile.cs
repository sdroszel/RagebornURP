using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float lifetime = 5f;         // Time before self-destruction
    [SerializeField] private int damage = 10;             // Damage dealt by the projectile
    [SerializeField] private AudioClip hitSound;          // Sound to play on hit
    [SerializeField] private AudioClip travelSound;       // Sound to play during travel
    [SerializeField] private float destroyDelay = 0.1f;   // Delay after collision before destruction
    [SerializeField] private GameObject soundObjectPrefab; // Prefab with AudioSource and ParticleSystem for hit effect

    private AudioSource audioSource;
    private bool hasHit = false; // Prevents multiple triggers on a single collision

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(travelSound);
        // Destroy the projectile after `lifetime` seconds if it doesn’t hit anything
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return; // Prevent multiple triggers
        hasHit = true;

        // Instantiate sound object to play hit sound and particle effect
        if (hitSound != null && soundObjectPrefab != null)
        {
            GameObject soundObject = Instantiate(soundObjectPrefab, transform.position, Quaternion.identity);

            // Play audio
            AudioSource soundAudioSource = soundObject.GetComponent<AudioSource>();
            if (soundAudioSource != null)
            {
                soundAudioSource.clip = hitSound;
                soundAudioSource.Play();
            }

            // Play particle effect
            ParticleSystem particleSystem = soundObject.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play();
            }

            // Destroy sound and particle effect object after both are complete
            Destroy(soundObject, Mathf.Max(hitSound.length, particleSystem.main.duration));
        }

        // Check if the projectile hit the player
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }

        // Destroy the projectile after the delay, even if it didn't hit the player
        Destroy(gameObject, destroyDelay);
    }
}
