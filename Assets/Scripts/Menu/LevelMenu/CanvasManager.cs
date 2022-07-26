using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public static bool isGamePaused;
    public LevelManager levelManager;
    public Choice choice;
    public GameSettingsManager settingsManager;

    private float choiceYCoordinate;

    public void Start()
    {
        choiceYCoordinate = choice.transform.localPosition.y;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            var canvas = GetComponent<Animator>();
            if (canvas.GetBool("isSettings"))
                settingsManager.CheckChanges();
            else
                canvas.SetBool("isPause", !canvas.GetBool("isPause"));
        }
    }

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

    public void SetChoicePosition()
    {
        choice.transform.localPosition = new Vector2(choice.transform.localPosition.x, choiceYCoordinate);
        choice.SetPosition(0);
        choice.NowPosition = choice.positions[0];
    }
}
