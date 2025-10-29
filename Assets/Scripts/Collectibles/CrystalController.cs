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
        fillCircle = UIManager.Instance.FillCircleImage;
    }

    void Rotate()
    {
        transform.Rotate(new Vector3(0, 0, 50) * Time.deltaTime);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<JammoPlayerController>().crystalCount < Constants.TotalPlayerCapacity)
            {
                UIManager.Instance.EInterection.SetActive(true);
                fillCircle.gameObject.SetActive(true);
            }
            else
            {
                UIManager.Instance.ShowToast("You can not hold more cystals!",3f);
                SoundManager.Instance.PlaySound(SoundNames.BuzzerSound, SoundType.Effect);
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
      
        if (other.gameObject.CompareTag("Player"))
        {
    
            if (interactAction.IsPressed() && other.gameObject.GetComponent<JammoPlayerController>().crystalCount < Constants.TotalPlayerCapacity)
            {
                if (holdTimer < holdDuration) // Makes sure holdTimer stays within the bounds of the duration and doesn't over-increment
                {
                    IncrementTimer();
                }
                else if (holdTimer >= holdDuration) // If the holdTime is completed, add a crystal to the player
                {
                    ClearFillCircle();
                    UIManager.Instance.EInterection.SetActive(false);
                    this.gameObject.SetActive(false);
                    other.gameObject.GetComponent<JammoPlayerController>().AddCrystals(1);
              
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
             fillCircle.gameObject.SetActive(false);
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
        if(UIManager.Instance!=null)
        UIManager.Instance.EInterection.SetActive(false);
        holdTimer = 0;
         fillCircle.gameObject.SetActive(false);
        fillCircle.fillAmount = 0f;
    }

    void Update()
    {
        Rotate();
    }
    void OnDisable()
    {
        ClearFillCircle();
    }
    void OnDestroy()
    {
        ClearFillCircle();
    }
}
