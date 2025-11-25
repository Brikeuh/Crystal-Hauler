using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private GameManager gameManager;
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
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
    }

    public void StartGame()
    {
        // Load the first level or the level selection scene
        gameManager.Load(GameManager.GameModeScene.LevelSelection);
    }

    public void QuitGame()
    {
        gameManager.Quit();
    }

    public void OpenSettings()
    {
        gameManager.LoadAsync(GameManager.GameModeScene.Settings);
    }
}
