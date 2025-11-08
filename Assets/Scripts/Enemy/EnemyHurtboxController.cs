using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class EnemyHurtboxController : MonoBehaviour
{
    private int damageAmount = 10;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Hit by Enemy Attack");
            other.GetComponent<PlayerController>().TakeDamage(damageAmount);
        }
    }
}
