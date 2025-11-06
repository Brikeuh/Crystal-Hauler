using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System;


[Serializable]
public class LevelModel
{
    public LevelState CurrentLevelState;
    public LevelState PreviousLevelState;
    public GameState GameState;
    public PanelType CurrentOpenedPanelType;

    public GameObject Player;
    public GameObject PlayerObj;
    public GameObject Environment;
    public int Coins = 0;
    public bool IsInfiniteMode = false;
    public int TargetCrystals;
    public int Score;

    public float CurrentTime = 0;
    public float TotalTimer = 360;
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public LevelModel CurrentLevelModel;



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void OnEnable()
    {
        EventManager.OnPanelOpen += OnPanelOpen;
        EventManager.OnPanelClose += OnPanelClose;
        EventManager.OnGameStateChange += OnGameStateChange;
        EventManager.OnLevelStateChange += OnLevelStateChange;
    }


    void OnDisable()
    {
        EventManager.OnPanelOpen -= OnPanelOpen;
        EventManager.OnPanelClose -= OnPanelClose;
        EventManager.OnGameStateChange -= OnGameStateChange;
        EventManager.OnLevelStateChange -= OnLevelStateChange;
    }
    public void OnLevelStateChange(LevelState state)
    {
        CurrentLevelModel.CurrentLevelState = state;
    }
    public void OnPanelOpen(PanelType panelType)
    {


        if (CurrentLevelModel.CurrentLevelState != LevelState.Panel)
        {
            CurrentLevelModel.PreviousLevelState = CurrentLevelModel.CurrentLevelState;
            CurrentLevelModel.CurrentLevelState = LevelState.Panel;
        }
        CurrentLevelModel.CurrentOpenedPanelType = panelType;

    }
    public void OnPanelClose(PanelType panelType)
    {

        LevelState previousState = CurrentLevelModel.CurrentLevelState;
        CurrentLevelModel.CurrentLevelState = CurrentLevelModel.PreviousLevelState;
        CurrentLevelModel.PreviousLevelState = previousState;
        CurrentLevelModel.CurrentOpenedPanelType = PanelType.None;
    }
    void Start()
    {
        UIManager.Instance.TargetTxt.text = "Target: " + GameManager.Instance.CurrentLevelModel.TargetCrystals;
        GameManager.Instance.CurrentLevelModel.CurrentTime = GameManager.Instance.CurrentLevelModel.TotalTimer;
    }

    // Update is called once per frame
    void Update()
    {

if(CurrentLevelModel.GameState == GameState.Running && Input.GetKeyDown(KeyCode.Escape))
        {
            EventManager.OnGameStateChange?.Invoke(GameState.Paused);
            
        }
    }
   
   
    
    public void OnGameStateChange(GameState state)
    {
        CurrentLevelModel.GameState = state;
        switch (state)
        {
            case GameState.Complete:
           
                Invoke(nameof(LevelCompleted), 3f);
                //UIManager.Instance.ConfettiParticles.Play();
             
                break;
            case GameState.Fail:
           
                //Invoke(nameof(LevelFailed), 4f);
                break;

           
        }
    }
    public void LevelCompleted()
    {
        ReferenceManager.Instance.GetPanel(PanelType.GamePlayPanel).Panel.SetActive(false);

        ReferenceManager.Instance.GetPanel(PanelType.LevelComplete).Panel.SetActive(true);
    }
    public void LevelFailed()
    {
        ReferenceManager.Instance.GetPanel(PanelType.GamePlayPanel).Panel.SetActive(false);
   
        ReferenceManager.Instance.GetPanel(PanelType.LevelFail).Panel.SetActive(true);
    }

    internal void IncrementScore(int Score)
    {
        CurrentLevelModel.Score += Score;
        if(CurrentLevelModel.Score>=CurrentLevelModel.TargetCrystals)
        {
            EventManager.OnGameStateChange?.Invoke(GameState.Complete);
        }
    }
}
