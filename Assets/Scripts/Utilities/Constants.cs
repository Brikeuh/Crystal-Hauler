using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum LevelState { MainMenu,Gameplay, Panel}
public enum GameState{None,Running,Fail,Complete, Paused }
public enum PanelType { None, IntroPanel, LevelFail, GamePlayPanel, LevelComplete, ToastPanel, LevelFailed,Pause}


public static class Constants
{

    public static int TotalPlayerCapacity = 5;

}
