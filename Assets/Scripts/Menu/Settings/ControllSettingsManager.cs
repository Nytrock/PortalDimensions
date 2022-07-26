using UnityEngine;

public class ControllSettingsManager : MonoBehaviour
{
    public Animator canvas;

    public void CheckChanges()
    {
        canvas.SetBool("isSettingsControll", !canvas.GetBool("isSettingsControll"));
    }
}
