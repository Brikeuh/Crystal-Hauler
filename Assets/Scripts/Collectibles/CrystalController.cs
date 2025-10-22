using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.IO;

public class CrystalController : MonoBehaviour
{
    InputAction interactAction;
    Image fillCircle;

    public static float MaxImageFill = 1f;
    public float holdDuration = 1f;
    private float holdTimer = 0f;

    private void Start()
    {
        interactAction = InputSystem.actions.FindAction("Player/Interact");
        fillCircle = GameObject.FindGameObjectWithTag("LoadCircle").GetComponent<Image>();
    }

    void Rotate()
    {
        transform.Rotate(new Vector3(0, 0, 50) * Time.deltaTime);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (interactAction.IsPressed())
            {
                if (holdTimer < holdDuration) // Makes sure holdTimer stays within the bounds of the duration and doesn't over-increment
                {
                    IncrementTimer();
                }
                else if (holdTimer >= holdDuration) // If the holdTime is completed, add a crystal to the player
                {
                    ClearFillCircle();
                    this.gameObject.SetActive(false);
                    other.gameObject.GetComponent<JammoPlayerController>().crystalCount++;
                }
            }
            else if (!interactAction.IsPressed())
            {
                if(holdTimer >= 0) // Same as above, but for  over-decrementing
                {
                    DecrementTimer();
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

    void Update()
    {
        Rotate();
    }   
}
