using UnityEngine;

public class ProjectileCrystal : MonoBehaviour
{
    [Header("Lifetime Settings")]
    public float maxLifetime = 5f;
    public float damage;

    private void Start()
    {
        // Destroy after max lifetime even if it doesn't hit anything
        Destroy(gameObject, maxLifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore collision with the object that spawned it
        if (other.CompareTag("Player"))
        {
            return; // Optional: ignore player collisions
        }
        else if (other.CompareTag("Enemy"))
        {
            Debug.Log($"Projectile hit: {other.gameObject.name}");
            other.gameObject.GetComponent<EnemyController>().TakeDamage(damage);
        }
        
        // Destroy the projectile
        Destroy(gameObject);
    }
}
