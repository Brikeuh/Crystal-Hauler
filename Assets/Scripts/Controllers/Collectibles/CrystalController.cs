using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CrystalController : MonoBehaviour
{
    InputAction interactAction;

    public static float MaxImageFill = 1f; // The maximum fill amount for the image
    public float holdDuration = 1f; // The duration the player needs to hold the interact button, not necessarily equal to MaxImageFill
    private float holdTimer = 0f;
    private float maxCrystals = 5f;

    [Header("Scriptable Objects")]
    [SerializeField] private FloatScriptableObject crystalCountSO;
    [SerializeField] private FloatScriptableObject fillCircleAmountSO;

    private void Start()
    {
        interactAction = InputSystem.actions.FindAction("Player/Interact");
    }

    void Rotate()
    {
        transform.Rotate(new Vector3(0, 50, 0) * Time.deltaTime);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && crystalCountSO.Value < maxCrystals)
        {
            if (interactAction.IsPressed())
            {
                if (holdTimer < MaxImageFill) // Makes sure holdTimer stays within the bounds of the duration and doesn't over-increment
                {
                    IncrementTimer();
                    other.gameObject.GetComponent<Animator>().SetBool("isPickingUp", true);
                }
                else if (fillCircleAmountSO.Value >= MaxImageFill) // If the holdTime is completed, add a crystal to the player
                {
                    Destroy(gameObject);
                    ClearFillCircle();
                    crystalCountSO.Value++;
                    other.gameObject.GetComponent<Animator>().SetBool("isPickingUp", false);
                }
            }
            else if (!interactAction.IsPressed())
            {
                if(holdTimer >= 0) // Same as above, but for  over-decrementing
                {
                    DecrementTimer();
                    other.gameObject.GetComponent<Animator>().SetBool("isPickingUp", false);
                }
            }
        }
        else if(other.gameObject.CompareTag("Player") && crystalCountSO.Value >= maxCrystals)
        {
            Debug.Log("Crystal Limit Reached!");
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
        fillCircleAmountSO.Value = 0f;
    }

    void Update()
    {
        Rotate();
    }   
}
