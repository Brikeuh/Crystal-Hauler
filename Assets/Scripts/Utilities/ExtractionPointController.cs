using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExtractionPointController : MonoBehaviour
{
    InputAction interactAction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactAction = InputSystem.actions.FindAction("Player/Interact");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<JammoPlayerController>().crystalCount > 0)
        {
            if (interactAction.IsPressed())
            {
                Debug.Log("test");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
