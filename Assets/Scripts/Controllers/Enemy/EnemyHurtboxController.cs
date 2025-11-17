using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class EnemyHurtboxController : MonoBehaviour
{
    private float damageAmount = 10f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Hit by Enemy Attack");
            other.GetComponent<Player>().TakeDamage(damageAmount);
        }
    }
}
