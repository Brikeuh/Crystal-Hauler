using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;


public static class EventManager
{
    public static UnityAction<PanelType> OnPanelOpen;
    public static UnityAction<PanelType> OnPanelClose;
    public static UnityAction<GameState> OnGameStateChange;
    public static UnityAction<LevelState> OnLevelStateChange;
    public static UnityAction<int> OnCrystalChanged;
    public static UnityAction<int> OnScoresChanged;
    
    public static UnityAction<float> OnHealthChanged;

}
