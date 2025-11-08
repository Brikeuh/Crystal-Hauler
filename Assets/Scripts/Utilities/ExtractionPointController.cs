using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ExtractionPointController : MonoBehaviour
{
    InputAction interactAction;
    Image fillCircle;

    public float MaxImageFill = 1f;
    public float holdDuration = 5f;
    private float holdTimer = 0f;
   
    void Start()
    {
        interactAction = InputSystem.actions.FindAction("Player/Interact");
        fillCircle = GameObject.FindGameObjectWithTag("LoadCircle").GetComponent<Image>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PlayerController>().crystalCount > 0)
        {
            if (interactAction.IsPressed())
            {
                if (holdTimer < MaxImageFill) // Makes sure holdTimer stays within the bounds of the duration and doesn't over-increment
                {
                    IncrementTimer();
                    other.gameObject.GetComponent<Animator>().SetBool("isExtracting", true);
                }
                else if (holdTimer >= MaxImageFill)
                {
                    ClearFillCircle();
                    other.gameObject.GetComponent<PlayerController>().crystalCount = 0;
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
        fillCircle.fillAmount = holdTimer / MaxImageFill;
    }

    void IncrementTimer()
    {
        holdTimer += Time.deltaTime / holdDuration;
        fillCircle.fillAmount = holdTimer / MaxImageFill;
    }

    void ClearFillCircle()
    {
        holdTimer = 0;
        fillCircle.fillAmount = 0f;
    }
}
