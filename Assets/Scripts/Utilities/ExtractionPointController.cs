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
        fillCircle = UIManager.Instance.FillCircleImage;
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
           if (other.gameObject.CompareTag("Player") )
            {
                if (other.gameObject.GetComponent<JammoPlayerController>().crystalCount > 0)
                {
                    fillCircle.gameObject.SetActive(true);
                    UIManager.Instance.EInterection.SetActive(true);
                }
                else
                {
                    UIManager.Instance.ShowToast("You do not have any crystals");
                    SoundManager.Instance.PlaySound(SoundNames.BuzzerSound, SoundType.Effect);
                }
                
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<JammoPlayerController>().crystalCount > 0)
        {
            if (interactAction.IsPressed())
            {
                if (holdTimer < MaxImageFill) // Makes sure holdTimer stays within the bounds of the duration and doesn't over-increment
                {
                    IncrementTimer();
                }
                else if (holdTimer >= MaxImageFill)
                {
                    ClearFillCircle();
                    other.gameObject.GetComponent<JammoPlayerController>().DepositCrystals();
                }
            }
            else if (!interactAction.IsPressed())
            {
                if (holdTimer >= 0) // Same as above, but for  over-decrementing
                {
                    DecrementTimer();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        { fillCircle.gameObject.SetActive(false);
            ClearFillCircle();
                 UIManager.Instance.EInterection.SetActive(false);
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
    { fillCircle.gameObject.SetActive(false);
        holdTimer = 0;
        fillCircle.fillAmount = 0f;
           UIManager.Instance.EInterection.SetActive(false);
    }
}
