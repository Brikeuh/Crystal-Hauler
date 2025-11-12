using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private FloatScriptableObject scoreSO;
    [SerializeField] private FloatScriptableObject crystalCountSO;
    [SerializeField] private FloatScriptableObject playerHealthSO;
    [SerializeField] private IntScriptableObject timerSO;

    private GameManager gameManager;
    private UIManager uiManager;
    private int levelDuration = 60;
    private void Awake()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager instance is null. Make sure a GameManager exists in the scene.");
        }
        else
        {
            gameManager = GameManager.Instance;
        }

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

        scoreSO.Value = 0;
        crystalCountSO.Value = 0;
        playerHealthSO.Value = 100f;
        timerSO.Value = levelDuration;

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine(StartCountdown());
    }

    void Update()
    {
        CheckGameState();
    }

    void CheckGameState()
    {
        if (scoreSO.Value >= 1000)
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
            Debug.Log("Coroutine Active");
            timerSO.Value--; // Decrease the value by 1
            yield return new WaitForSeconds(1f); // Wait for 1 second
        }
    }
}
