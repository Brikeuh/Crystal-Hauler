using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

using TMPro;
using System;

public class UIManager : MonoBehaviour
{

   
    public Image HealthImage;
    public TextMeshProUGUI SilverTxt;
    public TextMeshProUGUI ScoreTxt;
    public TextMeshProUGUI TargetTxt;

    public GameObject EInterection;
    public Image TimerImg;


   public void UpdateCrystalsText(int amount)
   {
       SilverTxt.text = ReferenceManager.Instance.PlayerController.crystalCount.ToString()+"/"+Constants.TotalPlayerCapacity;
   }


   


    public static UIManager Instance;

    public Image FillCircleImage;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void OnEnable()
    {
        EventManager.OnPanelOpen += OnPanelOpen;
        EventManager.OnPanelClose += OnPanelClose;
        EventManager.OnLevelStateChange += OnGameStateChange;
        EventManager.OnGameStateChange += OnGameStateChangeNew;
        EventManager.OnHealthChanged += UpdateHealth;
        EventManager.OnCrystalChanged += UpdateCrystalsText;
        EventManager.OnScoresChanged += UpdateCoins;
    }
    void OnDisable()
    {
        EventManager.OnPanelOpen -= OnPanelOpen;
        EventManager.OnPanelClose -= OnPanelClose;
        EventManager.OnLevelStateChange -= OnGameStateChange;
        EventManager.OnGameStateChange -= OnGameStateChangeNew;
        EventManager.OnHealthChanged -= UpdateHealth;
        EventManager.OnCrystalChanged -= UpdateCrystalsText;
    EventManager.OnScoresChanged -= UpdateCoins;
    }
    public void UpdateCoins(int Score)
    {

        GameManager.Instance.IncrementScore(Score);
        ScoreTxt.text = "Score: "+GameManager.Instance.CurrentLevelModel.Score.ToString();
    }
    public void OnGameStateChangeNew(GameState state)
    {
        switch (state)
        {
           case GameState.Paused:
                ReferenceManager.Instance.GetPanel(PanelType.Pause).Panel.SetActive(true);
                SoundManager.Instance.PlayButtonSound();
                break;

            case GameState.Running:
ReferenceManager.Instance.GetPanel(PanelType.GamePlayPanel).Panel.SetActive(true);
                break;
            case GameState.Fail:

               

                ReferenceManager.Instance.GetPanel(PanelType.LevelFail).Panel.SetActive(true);
                SoundManager.Instance.PlaySound(SoundNames.LevelFail, SoundType.Effect);
                break;
            case GameState.Complete:
ReferenceManager.Instance.GetPanel(PanelType.GamePlayPanel).Panel.SetActive(false);

                ReferenceManager.Instance.GetPanel(PanelType.LevelComplete).Panel.SetActive(true);
                SoundManager.Instance.PlaySound(SoundNames.LevelComplete, SoundType.Effect);
                break;
        }
    }
    public void OnGameStateChange(LevelState state)
    {
        switch (state)
        {
          

            case LevelState.Gameplay:
             ReferenceManager.Instance.GetPanel(PanelType.IntroPanel).Panel.SetActive(false);
                ReferenceManager.Instance.GetPanel(PanelType.GamePlayPanel).Panel.SetActive(true);
                 
          
                 break;

            case LevelState.MainMenu:
                GameManager.Instance.CurrentLevelModel.GameState = GameState.None;
               
                break;
        }
    }
    public void OnPanelOpen(PanelType panelType)
    {
        GameObject panel = ReferenceManager.Instance.GetPanel(panelType).Panel;
        if (panel != null)
        {
            panel.SetActive(true);
        }
    }

    public void OnPanelClose(PanelType panelType)
    {
        GameObject panel = ReferenceManager.Instance.GetPanel(panelType).Panel;
        if (panel != null)
        {
            panel.SetActive(false);
        }

    }
    

   
    void Update()
    {

        if(GameManager.Instance.CurrentLevelModel.GameState==GameState.Running)
        {

            TimerImg.fillAmount = GameManager.Instance.CurrentLevelModel.CurrentTime / GameManager.Instance.CurrentLevelModel.TotalTimer;
            GameManager.Instance.CurrentLevelModel.CurrentTime -= Time.deltaTime;
            if (GameManager.Instance.CurrentLevelModel.CurrentTime <= 0)
                EventManager.OnGameStateChange?.Invoke(GameState.Fail);

        }
    }
    void Start()
    {
        //if (Application.isEditor)
         //   PlayButtonClicked();

        UpdateCrystalsText(0);
    }
    public void UpdateCoins()
    {
        //CoinsTxt.text = EconomyManager.Instance.Coins.ToString();
    }
    public void UpdateHealth(float health)
    {
        HealthImage.fillAmount = health;
    }

    // Update is called once per frame

       public void PlayButtonClicked()
    {
        ReferenceManager.Instance.GetPanel(PanelType.IntroPanel).Panel.SetActive(false);
        EventManager.OnGameStateChange?.Invoke(GameState.Running);
        SoundManager.Instance.PlayButtonSound();
         Cursor.lockState = CursorLockMode.Locked;
    }
    public void SwitchTheCam()
    {
       
}
    public void OpenPrivacyPolicy()
    {
   
    }

    internal void ShowToast(string v, float duration =2,bool LetPreviousRunning=false)
    {

        StopCoroutine("ToastCoroutine");
        StartCoroutine(ToastCoroutine( v,duration));
    }
    public IEnumerator ToastCoroutine(string v, float Delay)
    {
        ReferenceManager.Instance.GetPanel(PanelType.ToastPanel).Txt.text = v;
        ReferenceManager.Instance.GetPanel(PanelType.ToastPanel).Panel.SetActive(true);
        yield return new WaitForSeconds(Delay);
        ReferenceManager.Instance.GetPanel(PanelType.ToastPanel).Panel.SetActive(false);
    }
    public void RetryScene()
    {
        SoundManager.Instance.PlayButtonSound();
       UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
