using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoSettingsManager : MonoBehaviour
{
    public Animator canvas;

    public void CheckChanges()
    {
        canvas.SetBool("isSettingsVideo", !canvas.GetBool("isSettingsVideo"));
    }
}
