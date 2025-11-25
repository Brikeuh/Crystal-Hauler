using UnityEngine;

public class LevelSelectionController : MonoBehaviour
{
    private GameManager gameManager;

    void Awake()
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

    public void LoadEasyLevel()
    {
        gameManager.Load(GameManager.GameModeScene.UI);
        gameManager.LoadAsync(GameManager.GameModeScene.EasyLevel);
    }

    public void LoadMediumLevel()
    {
        gameManager.Load(GameManager.GameModeScene.UI);
        gameManager.Load(GameManager.GameModeScene.MediumLevel);
    }

    public void LoadHardLevel()
    {
        gameManager.Load(GameManager.GameModeScene.UI);
        gameManager.Load(GameManager.GameModeScene.HardLevel);
    }

    public void LoadTutorial()
    {
        gameManager.Load(GameManager.GameModeScene.UI);
        gameManager.Load(GameManager.GameModeScene.Tutorial);
    }

    public void LoadMainMenu()
    {
        gameManager.Load(GameManager.GameModeScene.MainMenu);
    }
}
