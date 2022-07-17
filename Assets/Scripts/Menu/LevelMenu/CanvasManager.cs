using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public static bool isGamePaused;
    public LevelManager levelManager;
    public Choice choice;
    public void ReloadScene()
    {
        levelManager.RestartScene();
    }
    public void PauseGame()
    {
        Time.timeScale = 0;
        isGamePaused = true;
        choice.StartPauseWorking();
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
        isGamePaused = false;
        choice.StopPauseWorking();
    }
}
