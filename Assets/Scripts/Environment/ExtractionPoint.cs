using UnityEngine;

public class ExtractionPoint : MonoBehaviour
{
    public float MaxImageFill = 1f;
    public float holdDuration = 5f;
    private float holdTimer = 0f;

    [SerializeField] private FloatScriptableObject crystalCountSO;
    [SerializeField] private FloatScriptableObject scoreSO;
    [SerializeField] private FloatScriptableObject fillCircleAmountSO;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && crystalCountSO.Value > 0)
        {
            if (other.gameObject.GetComponent<Player>().InteractPressed)
            {
                if (holdTimer < MaxImageFill) // Makes sure holdTimer stays within the bounds of the duration and doesn't over-increment
                {
                    IncrementTimer();
                    other.gameObject.GetComponent<Player>().CanExtract = true;
                }
                else if (fillCircleAmountSO.Value >= MaxImageFill)
                {
                    ClearFillCircle();
                    scoreSO.Value += crystalCountSO.Value;
                    crystalCountSO.Value = 0;
                    other.gameObject.GetComponent<Player>().CanExtract = false;
                }
            }
            else if (!other.gameObject.GetComponent<Player>().InteractPressed)
            {
                if (holdTimer >= 0) // Same as above, but for  over-decrementing
                {
                    DecrementTimer();
                    other.gameObject.GetComponent<Player>().CanExtract = false;
                }
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
}
