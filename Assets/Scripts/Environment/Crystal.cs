using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Crystal : MonoBehaviour
{
    public static float MaxImageFill = 1f; // The maximum fill amount for the image
    public float holdDuration = 1f; // The duration the player needs to hold the interact button, not necessarily equal to MaxImageFill
    private float holdTimer = 0f;
    private float maxCrystals = 5f;

    [Header("Scriptable Objects")]
    [SerializeField] private FloatScriptableObject crystalCountSO;
    [SerializeField] private FloatScriptableObject fillCircleAmountSO;

    void Rotate()
    {
        transform.Rotate(new Vector3(0, 50, 0) * Time.deltaTime);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();

            if (player.CurrentStateName != "MovementState" && player.CurrentStateName != "FallingState" && crystalCountSO.Value < maxCrystals)
            {
                if (player.InteractPressed)
                {
                    if (holdTimer < MaxImageFill) // Makes sure holdTimer stays within the bounds of the duration and doesn't over-increment
                    {
                        IncrementTimer();
                        player.CanPickup = true;
                    }
                    else if (fillCircleAmountSO.Value >= MaxImageFill) // If the holdTime is completed, add a crystal to the player
                    {
                        SoundManager.Instance.PlaySound(SoundNames.CrystalPicked, SoundType.Effect, 0.7f, false);
                        Destroy(gameObject);
                        ClearFillCircle();
                        crystalCountSO.Value++;
                        player.CanPickup = false;
                    }
                }
                else if (!player.InteractPressed)
                {
                    if (holdTimer >= 0) // Same as above, but for over-decrementing
                    {
                        DecrementTimer();
                        player.CanPickup = false;
                    }
                }
            }
            else if (other.gameObject.CompareTag("Player") && crystalCountSO.Value >= maxCrystals)
            {
                Debug.Log("Crystal Limit Reached!");
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

    void Update()
    {
        Rotate();
    }   
}
