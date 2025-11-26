using UnityEngine;

public class DisableUIBehind : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject gameResultsPanel;
    [SerializeField] private GameObject pauseMenu;

    [Header("Scriptable Objects")]
    [SerializeField] private FloatScriptableObject levelPointGoalSO;
    [SerializeField] private FloatScriptableObject scoreSO;
    [SerializeField] private BoolValue levelEndedSO;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        gameResultsPanel.SetActive(false);
        pauseMenu.SetActive(false);
    }

    public void ReactivatePanels()
    {
        if (!levelEndedSO.Value)
        {
            pauseMenu.SetActive(true);
        }
        else if (levelEndedSO.Value)
        {
            gameResultsPanel.SetActive(true);
        }
    }
}
