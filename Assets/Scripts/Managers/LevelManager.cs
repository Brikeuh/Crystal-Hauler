using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private FloatScriptableObject scoreSO;
    [SerializeField] private FloatScriptableObject crystalCountSO;
    [SerializeField] private FloatScriptableObject playerHealthSO;
    [SerializeField] private FloatScriptableObject levelPointGoalSO;
    [SerializeField] private IntScriptableObject timerSO;
    [SerializeField] private IntScriptableObject enemiesDefeatedSO;
    [SerializeField] private BoolValue levelEndedSO;

    [Header("Level Attributes")]
    [SerializeField] private int levelDuration = 60;
    [SerializeField] private GameObject extractionPoints;

    private int targetScore;

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
        enemiesDefeatedSO.Value = 0;
        levelEndedSO.Value = false;
        levelPointGoalSO.Value = CalculateMaxScore(GetExtractionPointChildren());
        //Debug.Log(targetScore);

        SoundManager.Instance.PlayMainMenuSound();

        uiManager.SetTargetScore((int)levelPointGoalSO.Value);
        StartCoroutine(StartCountdown());
    }

    void Update()
    {
        CheckGameState();
    }

    void CheckGameState()
    {
        if (scoreSO.Value >= levelPointGoalSO.Value)
        {
            SoundManager.Instance.PlaySound(SoundNames.LevelWon, SoundType.BackGround, 0.7f, false);
            EndLevel(true); 
        }
        else if (playerHealthSO.Value <= 0f || timerSO.Value <= 0)
        {
            SoundManager.Instance.PlaySound(SoundNames.LevelLost, SoundType.BackGround, 0.7f, false);
            EndLevel(false);
        }
    }

    void EndLevel(bool result)
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        //SoundManager.Instance.StopSound(SoundType.BackGround);

        levelEndedSO.Value = true;
        uiManager.ShowGameResults(result);
        this.enabled = false; // Disable further updates
    }

    public int CalculateMaxScore(Transform[] extractionPoints)
    {
        int score = 0;
        for (int i = 0; i < extractionPoints.Length; i++)
        {
            score += extractionPoints[i].GetComponent<ExtractionPoint>().ExtractLimit;
        }
        return score;
    }

    public Transform[] GetExtractionPointChildren()
    {
        if (extractionPoints == null)
        {
            Debug.LogWarning("Target object is not assigned!");
            return new Transform[0];
        }

        List<Transform> children = new List<Transform>();
        Transform targetTransform = extractionPoints.transform;

        foreach (Transform child in targetTransform)
        {
            children.Add(child);
        }

        return children.ToArray();
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
