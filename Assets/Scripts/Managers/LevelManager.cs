using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private FloatScriptableObject scoreSO;
    [SerializeField] private FloatScriptableObject crystalCountSO;
    [SerializeField] private FloatScriptableObject playerHealthSO;
    [SerializeField] private IntScriptableObject timerSO;

    [Header("Level Attributes")]
    [SerializeField] private int levelDuration = 60;
    [SerializeField] private int targetScore = 10;

    private UIManager uiManager;

    private void Awake()
    {
        if (UIManager.Instance == null)
        {
            Debug.LogError("UIManager instance is null. Make sure a UIManager exists in the scene.");
        }
        else
        {
            uiManager = UIManager.Instance;
        }
    }

    void Start()
    {
        Debug.Log("Level Manager Started");

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;

        scoreSO.Value = 0;
        crystalCountSO.Value = 0;
        playerHealthSO.Value = 100f;
        timerSO.Value = levelDuration;

        uiManager.SetTargetScore(targetScore);
        StartCoroutine(StartCountdown());
    }

    void Update()
    {
        CheckGameState();
    }

    void CheckGameState()
    {
        if (scoreSO.Value >= targetScore)
        {
            EndLevel(true); 
        }
        else if (playerHealthSO.Value <= 0f || timerSO.Value <= 0)
        {
            EndLevel(false);
        }
    }

    void EndLevel(bool result)
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;

        uiManager.ShowGameResults(result);
        this.enabled = false; // Disable further updates
    }

    IEnumerator StartCountdown()
    {
        while (timerSO.Value > 0)
        {
            timerSO.Value--; // Decrease the value by 1
            yield return new WaitForSeconds(1f); // Wait for 1 second
        }
    }
}
