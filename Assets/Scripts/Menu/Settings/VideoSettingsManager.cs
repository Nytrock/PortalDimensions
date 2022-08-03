using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VideoSettingsManager : MonoBehaviour
{
    public Animator canvas;

    [Header("Дефолтные настройки")]
    [SerializeField] private int defaultResolution;
    [SerializeField] private int defaultMod;

    [Header("Настройка разрешения")]
    public List<string> resolutionsScreen;
    private int originallResolution;
    private bool isResolutionChange;
    public int resolutionId;
    public Button nextResolutionButton;
    public Button previousResolutionButton;
    public TextMeshProUGUI resolutionText;

    [Header("Настройка режима экрана")]
    public List<string> screenMods;
    private int originallMod;
    private bool isModsChange;
    public int modId;
    public Button nextModButton;
    public Button previousModButton;
    public TextMeshProUGUI modText;

    private void Start()
    {
        originallResolution = resolutionId;
        originallMod = modId;

        SetResolutionScreen();
        SetScreenMode(Screen.width, Screen.height);
    }

    public void CheckChanges()
    {
        var buttonFunc = canvas.GetComponent<ButtonFunctional>();
        if (isModsChange || isResolutionChange) {
            buttonFunc.SetConfirmPanel("VideoSettings");
        } else {
            buttonFunc.VideoSettings();
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
        resolutionId = originallResolution;
        SetResolutionScreen();

        modId = originallMod;

        canvas.SetBool("isSettingsVideo", false);
    }

    public void NextResolution()
    {
        if (resolutionId - 1 >= 0)
            resolutionId -= 1;
        else
            resolutionId = resolutionsScreen.Count - 1;
        SetResolutionScreen();
    }

    public void PreviousResolution()
    {
        if (resolutionId + 1 < resolutionsScreen.Count)
            resolutionId += 1;
        else
            resolutionId = 0;
        SetResolutionScreen();
    }

    public void NextMod()
    {
        if (modId + 1 < screenMods.Count)
            modId += 1;
        else
            modId = 0;
        SetScreenMode(Screen.width, Screen.height);
    }

    public void PreviousMod()
    {
        if (modId - 1 >= 0)
            modId -= 1;
        else
            modId = screenMods.Count - 1;
        SetScreenMode(Screen.width, Screen.height);
    }

    private void SetResolutionScreen()
    {
        resolutionText.text = resolutionsScreen[resolutionId];
        isResolutionChange = originallResolution != resolutionId;

        var resolution = resolutionsScreen[resolutionId].Split('x');
        int width = int.Parse(resolution[0]);
        int height = int.Parse(resolution[1]);
        SetScreenMode(width, height);
    }

    private void SetScreenMode(int width, int height)
    {
        modText.GetComponent<LocalizedText>().Localize(screenMods[modId]);
        isModsChange = originallMod != modId;

        nextResolutionButton.interactable = true;
        previousResolutionButton.interactable = true;

        switch (screenMods[modId])
        {
            case "Fullscreen": 
                Screen.SetResolution(1920, 1080, FullScreenMode.ExclusiveFullScreen);
                nextResolutionButton.interactable = false;
                previousResolutionButton.interactable = false;
                resolutionId = 0;
                resolutionText.text = resolutionsScreen[resolutionId];
                break;
            case "Windowed": Screen.SetResolution(width, height, FullScreenMode.Windowed); break;
            case "Borderless": Screen.SetResolution(width, height, FullScreenMode.FullScreenWindow); break;
        }
    }

    public void SetNewOriginall()
    {
        originallResolution = resolutionId;
        originallMod = modId;
    }

    public void SetChangesFalse()
    {
        isResolutionChange = false;
        isModsChange = false;
    }

    public void SetDefaults()
    {
        resolutionId = defaultResolution;
        SetResolutionScreen();

        modId = defaultMod;
        SetScreenMode(Screen.width, Screen.height);
    }
}
