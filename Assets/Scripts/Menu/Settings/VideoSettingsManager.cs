using System.Collections.Generic;
using UnityEngine;

public class VideoSettingsManager : MonoBehaviour
{
    public Animator canvas;

    [Header("Настройка разрешения")]
    public List<string> resolutionsScreen;
    public bool isResolutionChange;
    public int resolutionId;

    [Header("Настройка режима экрана")]
    public List<string> screenMods;
    public bool isModsChange;
    public int modId;

    public void CheckChanges()
    {
        if (isModsChange || isResolutionChange) {
            canvas.GetComponent<ButtonFunctional>().SetConfirmPanel("VideoSettings");
        } else {
            canvas.SetBool("isSettingsVideo", !canvas.GetBool("isSettingsVideo"));
        }
    }

    public void ConfirmCancel(bool value)
    {
        if (value)
            ReturnToNormal();
        canvas.SetBool("isConfirm", false);
    }

    private void ReturnToNormal()
    {
        canvas.SetBool("isSettingsVideo", false);
    }

    public void SetNewOriginall()
    {

    }

    public void SetChangesFalse()
    {

    }

    public void SetDefaults()
    {

    }
}
