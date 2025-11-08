using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");
        Time.timeScale = 1f;
        SoundManager.Instance.PlayButtonSound();
    }
}
