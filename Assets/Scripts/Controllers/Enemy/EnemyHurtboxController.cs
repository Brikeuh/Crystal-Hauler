using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class EnemyHurtboxController : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().TakeDamage(enemyController.AttackDamage);
        }
    }
}
