using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ExtractionPointController : MonoBehaviour
{
    InputAction interactAction;

    public float MaxImageFill = 1f;
    public float holdDuration = 5f;
    private float holdTimer = 0f;

    [SerializeField]
    private FloatScriptableObject crystalCount;
    [SerializeField]
    private FloatScriptableObject score;
    [SerializeField]
    private FloatScriptableObject fillCircleAmount;

    void Start()
    {
        interactAction = InputSystem.actions.FindAction("Player/Interact");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && crystalCount.Value > 0)
        {
            if (interactAction.IsPressed())
            {
                if (holdTimer < MaxImageFill) // Makes sure holdTimer stays within the bounds of the duration and doesn't over-increment
                {
                    IncrementTimer();
                    other.gameObject.GetComponent<Animator>().SetBool("isExtracting", true);
                }
                else if (fillCircleAmount.Value >= MaxImageFill)
                {
                    ClearFillCircle();
                    score.Value += crystalCount.Value * 100; // Each crystal is worth 100 points
                    crystalCount.Value = 0;
                    other.gameObject.GetComponent<Animator>().SetBool("isExtracting", false);
                }
            }
            else if (!interactAction.IsPressed())
            {
                if (holdTimer >= 0) // Same as above, but for  over-decrementing
                {
                    DecrementTimer();
                    other.gameObject.GetComponent<Animator>().SetBool("isExtracting", false);
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
        fillCircleAmount.Value = holdTimer / MaxImageFill;
    }

    void IncrementTimer()
    {
        holdTimer += Time.deltaTime / holdDuration;
        fillCircleAmount.Value = holdTimer / MaxImageFill;
    }

    void ClearFillCircle()
    {
        holdTimer = 0;
        fillCircleAmount.Value = 0f;
    }
}
