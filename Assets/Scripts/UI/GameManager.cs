using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Player Object Reference")]
    public GameObject player;

    [Header("UI Object References")]
    public GameObject introUI;
    public GameObject winUI;

    private bool isGameWon; 
    private bool isGameLost;

    private enum GameState { Intro, InProgress, Won, Lost }
    private GameState currentState = GameState.Intro;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isGameWon = false;
        isGameLost = false;
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // State behaviors
        switch (currentState)
        {
            case GameState.Intro:
                Introduction();
                break;
            case GameState.InProgress:
                InProgress();
                break;
            case GameState.Won:
                GameIsWon();
                break;
            case GameState.Lost:
                GameIsLost();
                break;
            default:
                break;
        }
    }

    void Introduction()
    {
        introUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
    }

    void InProgress()
    {
        introUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;

        if (player.GetComponent<PlayerController>().score >= 10)
        {
            currentState = GameState.Won;
        }
        else if (isGameLost)
        {
            currentState = GameState.Lost;
        }
    }

    void GameIsWon()
    {
        Transform childTransform = winUI.transform.GetChild(0); 
        childTransform.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
    }

    void GameIsLost()
    {
        Transform childTransform = winUI.transform.GetChild(1);
        childTransform.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
    }

    public void StartGame()
    {
        currentState = GameState.InProgress;
    }

    public void WinGame()
    {
        isGameWon = true;
    }

    public void LoseGame()
    {
        isGameLost = true;
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

}
