using UnityEngine;

public class ExtractionPoint : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private FloatScriptableObject crystalCountSO;
    [SerializeField] private FloatScriptableObject scoreSO;
    [SerializeField] private FloatScriptableObject fillCircleAmountSO;
    [SerializeField] private FloatScriptableObject playerHealthSO;
    [SerializeField] private IntScriptableObject timerSO;

    [Header("Extraction Settings")]
    [SerializeField] private float extractLimit = 5f;
    [SerializeField] private PowerupType powerUp;

    private enum PowerupType { None, Speed, Time, Damage, Health }

    public Material newMaterial;

    private float holdDuration;
    private float MaxImageFill = 1f;
    private float holdTimer = 0f;
    private float crystalsExtracted = 0f;
    private GameObject cave;
    private MeshRenderer caveMeshRenderer;
    private void Start()
    {
        cave = this.transform.GetChild(1).gameObject;
        caveMeshRenderer = cave.GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        holdDuration = crystalCountSO.Value;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();

            if (player.CurrentStateName != "MovementState" && player.CurrentStateName != "FallingState" && crystalCountSO.Value > 0)
            {
                if (player.InteractPressed)
                {
                    if (holdTimer < MaxImageFill) // Makes sure holdTimer stays within the bounds of the duration and doesn't over-increment
                    {
                        IncrementTimer();
                        player.CanExtract = true;
                    }
                    else if (fillCircleAmountSO.Value >= MaxImageFill)
                    {
                        ClearFillCircle();
                        scoreSO.Value += crystalCountSO.Value;
                        crystalsExtracted += crystalCountSO.Value;
                        crystalCountSO.Value = 0;
                        player.CanExtract = false;
                    }
                }
                else if (!player.InteractPressed)
                {
                    if (holdTimer >= 0) // Same as above, but for  over-decrementing
                    {
                        DecrementTimer();
                        player.CanExtract = false;
                    }
                }
            }

            if (crystalsExtracted >= extractLimit)
            {
                this.GetComponent<Collider>().enabled = false;
                caveMeshRenderer.material = newMaterial;
                ApplyPowerup(player);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ClearFillCircle();
        }

    }
    void DecrementTimer()
    {
        holdTimer -= Time.deltaTime / holdDuration;
        fillCircleAmountSO.Value = holdTimer / MaxImageFill;
    }

    void IncrementTimer()
    {
        holdTimer += Time.deltaTime / holdDuration;
        fillCircleAmountSO.Value = holdTimer / MaxImageFill;
    }

    void ClearFillCircle()
    {
        holdTimer = 0f;
        fillCircleAmountSO.Value = 0f;
    }

    void ApplyPowerup(Player player)
    {
        switch (powerUp)
        {
            case PowerupType.Speed:
                player.MoveSpeed *= 1.25f;
                break;
            case PowerupType.Damage:
                player.AttackDamage *= 2f;
                break;
            case PowerupType.Health:
                playerHealthSO.Value = player.MaxHealth;
                break;
            case PowerupType.Time:
                timerSO.Value += 30;
                break;
            default:
                break;
        }
    }
}
