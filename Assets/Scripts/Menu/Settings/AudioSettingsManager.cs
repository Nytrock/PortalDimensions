using UnityEngine;

public class AudioSettingsManager : MonoBehaviour
{
    public Animator canvas;

    public void CheckChanges()
    {
        canvas.SetBool("isSettingsAudio", !canvas.GetBool("isSettingsAudio"));
    }
}
