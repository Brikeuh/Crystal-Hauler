using UnityEngine;

public class MissionPanel : MonoBehaviour
{
    void OnEnable()
    {
        Time.timeScale = 0f; // Pause the game
    }
    void OnDisable()
    {
        Time.timeScale = 1;
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
