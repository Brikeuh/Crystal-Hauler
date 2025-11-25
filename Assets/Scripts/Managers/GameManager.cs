using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameModeScene
    {
        MainMenu,
        UI,
        LevelSelection,
        Settings,
        Tutorial,
        EasyLevel,
        MediumLevel,
        HardLevel,
        GameResults
    }

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
            DontDestroyOnLoad(gameObject); // Optional: Persist across scenes
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Load(GameModeScene.MainMenu);
        LoadAsync(GameModeScene.UI);
    }

    public void Load(GameModeScene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }

    public void LoadAsync(GameModeScene scene)
    {
        SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Additive);
    }

    public void UnloadAsync(GameModeScene scene)
    {
        SceneManager.UnloadSceneAsync(scene.ToString());
    }

    public void ReloadCurrentScene() // Reloads all currently loaded scenes
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the active scene
        for (int i = 1; i < SceneManager.loadedSceneCount; i++) // Start from 1 to skip the active scene
        {
            Scene scene = SceneManager.GetSceneAt(i);
            SceneManager.LoadScene(scene.name, LoadSceneMode.Additive);
        }
    }

    public bool GetScene(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
