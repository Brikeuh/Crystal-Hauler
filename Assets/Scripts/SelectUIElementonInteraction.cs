using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectUIElementOnInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private Selectable element;

    [Header("Visualization")]
    [SerializeField] private bool showVisualization;
    [SerializeField] private Color navigationColour = Color.cyan;

    private void OnDrawGizmos()
    {
        if (!showVisualization)
        {
            return;
        }
        if (element == null)
        {
            return;
        }

        Gizmos.color = navigationColour;
        Gizmos.DrawLine(gameObject.transform.position, element.gameObject.transform.position);
    }

    public void JumpToElement()
    {
        if (eventSystem == null)
        {
            Debug.Log("Event System is not referenced or does not exist in the current context", this);
        }
        if (element == null)
        {
            Debug.Log("Element to jump to is not referenced or does not exist in the current context", this);
        }

        eventSystem.SetSelectedGameObject(element.gameObject);
    }
}
