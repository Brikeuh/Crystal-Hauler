using UnityEngine;

public class ProjectileCrystal : MonoBehaviour
{
    [Header("Lifetime Settings")]
    [SerializeField] private float maxLifetime = 5f;
    [SerializeField] private float attackDamage = 25f;

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
            other.gameObject.GetComponent<EnemyController>().TakeDamage(attackDamage);
        }
        
        // Destroy the projectile
        Destroy(gameObject);
    }
}
