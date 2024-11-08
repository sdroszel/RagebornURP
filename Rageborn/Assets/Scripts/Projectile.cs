using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float lifetime = 5f; // Time before self-destruction
    [SerializeField] private int damage = 10;     // Damage dealt by the projectile

    private void Start()
    {
        // Destroy the projectile after `lifetime` seconds
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the projectile hit the player
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        
    }
}
