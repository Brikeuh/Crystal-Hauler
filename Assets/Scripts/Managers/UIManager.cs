using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Scriptable Objects")]
    [SerializeField] private FloatScriptableObject scoreSO;
    [SerializeField] private FloatScriptableObject crystalCountSO;
    [SerializeField] private FloatScriptableObject playerHealthSO;
    [SerializeField] private FloatScriptableObject playerStaminaSO;
    [SerializeField] private FloatScriptableObject fillCircleAmountSO;
    [SerializeField] private IntScriptableObject timerSO;
    [SerializeField] private IntScriptableObject enemiesDefeatedSO;

    [Header("HUD Elements")]
    public GameObject HUD;
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI targetScoreText;
    public TextMeshProUGUI timerText;
    public GameObject loadCircle;
    public GameObject playerHealthProgressBar;
    public GameObject CrystalParentGameObject;
    public GameObject StaminaBarGameObject;
    public Image CurrentStaminaBarImage;

    [Header("Game Results UI References")]
    public GameObject GameResultsPanel;
    public GameObject GameWinPanel;
    public GameObject GameLosePanel;
    public GameObject postGameStats;
    public TextMeshProUGUI timeRemainingResults;
    public TextMeshProUGUI healthRemainingResults;
    public TextMeshProUGUI enemiesDefeatedResults;
    public TextMeshProUGUI crystalsExtractedResults;

    [Header("Canvas References")]
    public GameObject UICanvas;
    public GameObject MainMenuCanvas;
    public GameObject LevelSelectionCanvas;

    [Header("Select UI References")]
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private Selectable pauseMenuResumeButtonElement;

    [Header("Misc UI Elements")]
    public GameObject PauseMenu;
    public GameObject GameQuitModal;
    public GameObject EventSystem;

    private GameObject[] childObjects;
    private Image loadCircleImage;
    private Image playerHealthProgressBarImage;
    private GameManager gameManager;
    private bool isPaused = false;
    private int maxCrystals = 5;

    private InputAction pauseInput;
    private InputAction jumpInput;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // Destroy duplicate instances
            Destroy(gameObject);
        }
        else
        {
            Instance = this; // Assign the current instance
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager instance is null. Make sure a GameManager exists in the scene.");
        }
        else
        {
            gameManager = GameManager.Instance;
        }

        if (scoreSO == null || crystalCountSO == null || playerHealthSO == null)
        {
            Debug.LogError("One or more ScriptableObjects are not assigned in the inspector.");
            return;
        }
    }

    void Start()
    {
        loadCircleImage = loadCircle.GetComponent<Image>();
        playerHealthProgressBarImage = playerHealthProgressBar.GetComponent<Image>();
        childObjects = new GameObject[maxCrystals];

        UICanvas.SetActive(false);
        MainMenuCanvas.SetActive(false);
        LevelSelectionCanvas.SetActive(false);

        EventSystem.SetActive(true);

        pauseInput = InputSystem.actions.FindAction("UI/PauseGame");
        jumpInput = InputSystem.actions.FindAction("Player/Jump");

        if (gameManager.GetScene("MainMenu"))
        {
            MainMenuCanvas.SetActive(true);
        }
        else if (gameManager.GetScene("EasyLevel") || gameManager.GetScene("MediumLevel") || gameManager.GetScene("HardLevel"))
        {   
            UICanvas.SetActive(true);
            HUD.SetActive(true);
        }

        SetupCrystalCountUI();
    }

    // Update is called once per frame
    void Update()
    {
        currentScoreText.text = scoreSO.Value.ToString();
        DisplayCrystals();
        playerHealthProgressBarImage.fillAmount = playerHealthSO.Value / 100f;
        DisplayTimer();
        loadCircleImage.fillAmount = fillCircleAmountSO.Value;
        CurrentStaminaBarImage.fillAmount = playerStaminaSO.Value / 5f;

        if (pauseInput.WasReleasedThisFrame())
        {
            TogglePauseMenu();
        }

        if (playerStaminaSO.Value >= 5f)
        {
            StaminaBarGameObject.SetActive(false);
        }
        else
        {
            StaminaBarGameObject.SetActive(true);
        }
    }

   
    public void SetTargetScore(int value)
    {
        targetScoreText.text = value.ToString();
    }

    public void ShowGameResults(bool result)
    {
        HUD.SetActive(false);

        UICanvas.SetActive(true);
        GameResultsPanel.SetActive(true);

        if (result)
        {
            GameWinPanel.SetActive(true);
        }
        else if (!result)
        {
            GameLosePanel.SetActive(true);
        }

        postGameStats.SetActive(true);

        timeRemainingResults.text = "Time Remaining: " + timerSO.Value + " seconds";
        healthRemainingResults.text = "Health Remaining: " + playerHealthSO.Value;
        enemiesDefeatedResults.text = "Enemies Defeated: " + enemiesDefeatedSO.Value;
        crystalsExtractedResults.text = "Crystals Extracted: " + scoreSO.Value;
    }
    #region Public Helper Functions
    public void StartGame()
    {
        gameManager.UnloadAsync(GameManager.GameModeScene.MainMenu);
        MainMenuCanvas.SetActive(false);
        LevelSelectionCanvas.SetActive(true);
    }
    public void OpenSettings()
    {
        gameManager.LoadAsync(GameManager.GameModeScene.Settings);
    }
    public void QuitGame()
    {
        gameManager.Quit();
    }
    public void RestartLevel()
    {
        gameManager.ReloadCurrentScene();  
    }

    public void ContinueToNextLevel()
    {
        //gameManager.LoadNextLevel();
    }

    public void LoadEasyLevel()
    {
        LevelSelectionCanvas.SetActive(false);
        UICanvas.SetActive(true);
        HUD.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        gameManager.LoadAsync(GameManager.GameModeScene.EasyLevel);
    }

    public void LoadMediumLevel()
    {
        LevelSelectionCanvas.SetActive(false);
        UICanvas.SetActive(true);
        HUD.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        gameManager.LoadAsync(GameManager.GameModeScene.MediumLevel);
    }

    public void LoadHardLevel()
    {
        LevelSelectionCanvas.SetActive(false);
        UICanvas.SetActive(true);
        HUD.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        gameManager.LoadAsync(GameManager.GameModeScene.HardLevel);
    }

    public void LoadTutorial()
    {
        LevelSelectionCanvas.SetActive(false);
        UICanvas.SetActive(true);
        HUD.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        gameManager.LoadAsync(GameManager.GameModeScene.Tutorial);
    }

    public void LoadMainMenu()
    {
        LevelSelectionCanvas.SetActive(false);

        MainMenuCanvas.SetActive(true);
        gameManager.LoadAsync(GameManager.GameModeScene.MainMenu);
    }

    public void QuitToMainMenu() // Only use from the modal
    {
        GameQuitModal.SetActive(false);
        TogglePauseMenu();
        UICanvas.SetActive(false);
        HUD.SetActive(false);
        gameManager.UnloadAsync(GameManager.GameModeScene.EasyLevel);

        MainMenuCanvas.SetActive(true);
        gameManager.LoadAsync(GameManager.GameModeScene.MainMenu);
        Cursor.lockState = CursorLockMode.None;
    }

    public void QuitOptions()
    {
        GameQuitModal.SetActive(true);
    }
    public void ExitQuitModal()
    {
        GameQuitModal.SetActive(false);
    }

    public void TogglePauseMenu()
    {
        isPaused = !isPaused;
        jumpInput.Disable();
        if (isPaused)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            eventSystem.SetSelectedGameObject(pauseMenuResumeButtonElement.gameObject);
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            jumpInput.Enable();
        }
        PauseMenu.SetActive(isPaused);
    }
    #endregion

    #region Private Helper Functions
    private void DisplayTimer()
    {
        int minutes = timerSO.Value / 60;
        int seconds = timerSO.Value % 60;
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void DisplayCrystals()
    {
        for (int i = 0; i < maxCrystals; i++)
        {
            if (crystalCountSO.Value >= i + 1)
            {
                childObjects[i].SetActive(true);
            }
            else
            {
                childObjects[i].SetActive(false);
            }
        }
    }

    private void SetupCrystalCountUI() // Sets up an array with the child crystal game objects in the UI
    {
        int childCount = CrystalParentGameObject.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            childObjects[i] = CrystalParentGameObject.transform.GetChild(i).gameObject;
            childObjects[i].SetActive(false);

        }
    }
    #endregion

    public void JumpToElement()
    {
        if (eventSystem == null)
        {
            Debug.Log("Event System is not referenced or does not exist in the current context", this);
        }
        if (pauseMenuResumeButtonElement == null)
        {
            Debug.Log("Element to jump to is not referenced or does not exist in the current context", this);
        }

        eventSystem.SetSelectedGameObject(pauseMenuResumeButtonElement.gameObject);
    }
}
