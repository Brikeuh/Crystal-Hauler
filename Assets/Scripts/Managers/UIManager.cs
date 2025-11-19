using TMPro;
using UnityEngine.UI;
using UnityEngine;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private FloatScriptableObject scoreSO;
    [SerializeField] private FloatScriptableObject crystalCountSO;
    [SerializeField] private FloatScriptableObject playerHealthSO;
    [SerializeField] private FloatScriptableObject fillCircleAmountSO;
    [SerializeField] private IntScriptableObject timerSO;

    [Header("HUD Elements")]
    public GameObject HUD;
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI targetScoreText;
    public TextMeshProUGUI timerText;
    public GameObject loadCircle;
    public GameObject playerHealthProgressBar;

    [Header("Game Results UI Panels")]
    public GameObject GameWinPanel;
    public GameObject GameLosePanel;
    public GameObject GameQuitModal;
    public GameObject PauseMenu;
    public GameObject CrystalParentGameObject;

    private GameObject[] childObjects;
    private Image loadCircleImage;
    private Image playerHealthProgressBarImage;
    private GameManager gameManager;
    private bool isPaused = false;
    private int maxCrystals = 5;

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

        GameWinPanel.SetActive(false);
        GameLosePanel.SetActive(false);
        GameQuitModal.SetActive(false);
        PauseMenu.SetActive(false);

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

        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePauseMenu();
        }
    }

    void DisplayTimer()
    {
        int minutes = timerSO.Value / 60;
        int seconds = timerSO.Value % 60;
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void SetTargetScore(int value)
    {
        targetScoreText.text = value.ToString();
    }

    public void ShowGameResults(bool result)
    {
        HUD.SetActive(false);

        if (result)
        {
            GameWinPanel.SetActive(true);
        }
        else if (!result)
        {
            GameLosePanel.SetActive(true);
        }
    }

    public void TogglePauseMenu()
    {
        isPaused = !isPaused;
        if(isPaused)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
        }
        PauseMenu.SetActive(isPaused);
    }

    public void RestartLevel()
    {
        gameManager.ReloadCurrentScene();
    }

    public void ContinueToNextLevel()
    {
        //gameManager.LoadNextLevel();
    }

    public void QuitOptions()
    {
        GameQuitModal.SetActive(true);
    }

    public void ExitQuitModal()
    {
        GameQuitModal.SetActive(false);
    }

    public void QuitToMainMenu()
    {
        gameManager.Load(GameManager.GameModeScene.MainMenu);
    }

    public void QuitGame()
    {
        gameManager.Quit();
    }

    void DisplayCrystals()
    {
        for (int i = 0; i < maxCrystals; i++)
        {
            if(crystalCountSO.Value >= i + 1) 
            {
                childObjects[i].SetActive(true);
            }
            else
            {
                childObjects[i].SetActive(false);
            }
        }
    }
    void SetupCrystalCountUI() // Sets up an array with the child crystal game objects in the UI
    {
        int childCount = CrystalParentGameObject.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            childObjects[i] = CrystalParentGameObject.transform.GetChild(i).gameObject;
            childObjects[i].SetActive(false);

        }
    }

}
