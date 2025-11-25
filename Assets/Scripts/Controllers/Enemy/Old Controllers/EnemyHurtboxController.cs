using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class EnemyHurtboxController : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;
    private TakeDamage playerHitVolume;

    private void Start()
    {
        playerHitVolume = GameObject.FindWithTag("PostProcessingVolume").GetComponent<TakeDamage>();   
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().TakeDamage(enemyController.AttackDamage);
            playerHitVolume.DisplayDamageEffect();
        }
    }
}
