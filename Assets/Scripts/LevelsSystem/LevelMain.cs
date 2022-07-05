using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMain : MonoBehaviour
{
    public void RestartLevel()
    {
        var save = GameObject.Find("Save").GetComponent<Save>();
        if (save.AutoRestart)
            Debug.Log("Auto Restart");
        else
            Debug.Log("Menu Restart");
    }
}
