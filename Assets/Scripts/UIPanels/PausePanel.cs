using UnityEngine;

public class PausePanel : MonoBehaviour
{
    void OnEnable()
    {
        Time.timeScale = 0f; // Pause the game
    }
    void OnDisable()
    {
        Time.timeScale = 1;
    }
    public void Resume()
    {
        EventManager.OnGameStateChange?.Invoke(GameState.Running);
        this.gameObject.SetActive(false);

    }
    public void Retry()
    {
UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    public void Exit()
    {
      UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
