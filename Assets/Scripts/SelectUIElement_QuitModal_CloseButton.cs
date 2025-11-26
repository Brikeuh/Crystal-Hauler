using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Rendering.FilterWindow;

public class SelectUIElementQuitModalCloseButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private Selectable gameResultsWinQuitElement;
    [SerializeField] private Selectable gameResultsLoseQuitElement;
    [SerializeField] private Selectable pauseMenuQuitElement;
    [SerializeField] private FloatScriptableObject levelPointGoalSO;
    [SerializeField] private FloatScriptableObject scoreSO;
    [SerializeField] private BoolValue levelEndedSO;



    public void JumpToElement()
    {
        if (!levelEndedSO.Value)
        {
            eventSystem.SetSelectedGameObject(pauseMenuQuitElement.gameObject);
        }
        else if (levelEndedSO.Value && scoreSO.Value < levelPointGoalSO.Value)
        {
            eventSystem.SetSelectedGameObject(gameResultsLoseQuitElement.gameObject);
        }
        else if (levelEndedSO.Value && scoreSO.Value >= levelPointGoalSO.Value)
        {
            eventSystem.SetSelectedGameObject(gameResultsWinQuitElement.gameObject);
        }

    }
}
