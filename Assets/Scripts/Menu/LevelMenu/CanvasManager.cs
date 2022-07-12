using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public LevelManager levelManager;
    public void ReloadScene()
    {
        levelManager.RestartScene();
    }
}
