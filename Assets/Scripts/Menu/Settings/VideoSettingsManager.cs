using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VideoSettingsManager : MonoBehaviour
{
    public bool lightVersion;
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

    [Header("Текстуры и камеры")]
    [SerializeField] private RenderTexture[] textures;
    [SerializeField] private Camera[] renderCameras;

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
        if (!lightVersion) {
            resolutionText.text = resolutionsScreen[resolutionId];
            isResolutionChange = originallResolution != resolutionId;
        }

        var resolution = resolutionsScreen[resolutionId].Split('x');
        int width = int.Parse(resolution[0]);
        int height = int.Parse(resolution[1]);
        SetScreenMode(width, height);
        try {
            StartCoroutine(Textures());
        } catch {

        }
    }

    private void SetScreenMode(int width, int height)
    {
        if (!lightVersion) {
            modText.GetComponent<LocalizedText>().Localize(screenMods[modId]);
            isModsChange = originallMod != modId;

            nextResolutionButton.interactable = true;
            previousResolutionButton.interactable = true;

            if (screenMods[modId] == "Fullscreen") {
                nextResolutionButton.interactable = false;
                previousResolutionButton.interactable = false;
                resolutionId = 0;
                resolutionText.text = resolutionsScreen[resolutionId];
            }
        }
        if (screenMods[modId] == "Fullscreen"){
            width = 1920;
            height = 1080;
        }

        foreach (RenderTexture texture in textures) {
            texture.Release();
            texture.width = width;
            texture.height = height;
            texture.Create();
        }

        switch (screenMods[modId]) {
            case "Fullscreen": Screen.SetResolution(width, height, FullScreenMode.ExclusiveFullScreen); StartCoroutine(Textures()); break;
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

    IEnumerator Textures()
    {
        foreach (Camera camera in renderCameras)
            camera.targetTexture = null;
        yield return new WaitForSeconds(Time.deltaTime);
        for (int i = 0; i < renderCameras.Length; i++)
            renderCameras[i].targetTexture = textures[i];
    }
}
