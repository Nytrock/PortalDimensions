using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingsManager : MonoBehaviour
{
    public Animator canvas;

    [Header("Дефолтные настройки")]
    [SerializeField] private int defaultLanguage;
    [SerializeField] private int defaultCursor;
    [SerializeField] private bool defaultAuto;
    [SerializeField] private bool defaultFps;
    [SerializeField] private bool defaultConfirm;

    [Header("Настройки локализации")]
    public LocalizationManager localization;
    public int originallyLanguage;
    private bool isLanguageChange;

    [Header("Настройки курсора")]
    public List<Texture2D> cursorsTextures;
    public RawImage cursorImage;
    public int cursorId;
    private bool isCursorChange;
    private int originalCursor;

    [Header("Настройки авторестарта")]
    public Toggle autoManager;
    public bool autorestart;
    private bool originallyAuto;
    private bool isAutoChange;

    [Header("Настройки счётчика fps")]
    public Toggle fpsManager;
    public FpsCounter fpsCounter;
    public bool fpsShowing;
    private bool originallyFps;
    private bool isFpsChange;

    [Header("Настройки подтверждения выхода")]
    public Toggle confirmManager;
    public bool confirm;
    private bool originallyConfirm;
    private bool isConfirmChange;

    public void Start()
    {
        ChangeCursorTexture();
        SetNewOriginall();
        SetChangesFalse();

        autorestart = originallyAuto;
        confirm = originallyConfirm;
        fpsShowing = originallyFps;
    }
    public void NextLanguage()
    {
        int newId = 0;
        if (LocalizationManager.SelectedLanguage + 1 < Save.NumberLanguages)
            newId = LocalizationManager.SelectedLanguage + 1;
        localization.SetLanguage(newId);
        isLanguageChange = originallyLanguage != newId;
    }

    public void PreviousLanguage()
    {
        int newId = Save.NumberLanguages - 1;
        if (LocalizationManager.SelectedLanguage - 1 >= 0)
            newId = LocalizationManager.SelectedLanguage - 1;
        localization.SetLanguage(newId);
        isLanguageChange = originallyLanguage != newId;
    }

    public void ChangeAutorestart()
    {
        autorestart = !autorestart;
        isAutoChange = autorestart != originallyAuto;
    }

    public void ChangeConfirmNeed()
    {
        confirm = !confirm;
        isConfirmChange = confirm != originallyConfirm;
    }
    public void ChangeFps()
    {
        fpsShowing = !fpsShowing;
        isFpsChange = fpsShowing !=  originallyFps;
        fpsCounter.ChangeWorking(fpsShowing);
    }

    public void NextCursor()
    {
        if (cursorId + 1 < cursorsTextures.Count)
            cursorId += 1;
        else
            cursorId = 0;
        ChangeCursorTexture();
    }

    public void PreviousCursor()
    {
        if (cursorId - 1 >= 0)
            cursorId -= 1;
        else
            cursorId = cursorsTextures.Count - 1;
        ChangeCursorTexture();
    }

    private void ChangeCursorTexture()
    {
        CursorSeeker.cursorSeeker.cursorId = cursorId;
        Cursor.SetCursor(cursorsTextures[cursorId], Vector2.zero, CursorMode.Auto);
        isCursorChange = cursorId != originalCursor;
        cursorImage.texture = cursorsTextures[cursorId];
    }

    public void CheckChanges()
    {
        var buttonFunc = canvas.GetComponent<ButtonFunctional>();
        if (isLanguageChange || isAutoChange || isFpsChange || isConfirmChange || isCursorChange) {
            buttonFunc.SetConfirmPanel("GameSettings");
        } else {
            buttonFunc.GameSettings();
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
        SetChangesFalse();

        localization.SetLanguage(originallyLanguage);

        cursorId = originalCursor;
        ChangeCursorTexture();

        fpsManager.isOn = originallyFps;
        fpsCounter.ChangeWorking(originallyFps);

        autoManager.isOn = originallyAuto;

        confirmManager.isOn = originallyConfirm;

        canvas.SetBool("isSettingsGame", false);
    }

    public void SetNewOriginall()
    {
        originallyAuto = autoManager.isOn;
        originallyConfirm = confirmManager.isOn;
        originallyFps = fpsManager.isOn;
        originallyLanguage = LocalizationManager.SelectedLanguage;
        originalCursor = cursorId;
    }

    public void SetChangesFalse()
    {
        isAutoChange = false;
        isConfirmChange = false;
        isFpsChange = false;
        isCursorChange = false;
    }

    public void SetDefaults()
    {
        localization.SetLanguage(defaultLanguage);
        isLanguageChange = originallyLanguage != LocalizationManager.SelectedLanguage;

        cursorId = defaultCursor;
        ChangeCursorTexture();

        autorestart = defaultAuto;
        autoManager.isOn = defaultAuto;
        isAutoChange = autorestart != originallyAuto;

        confirm = defaultConfirm;
        confirmManager.isOn = defaultConfirm;
        isConfirmChange = confirm != originallyConfirm;

        fpsShowing = defaultFps;
        isFpsChange = fpsShowing != originallyFps;
        fpsManager.isOn = defaultFps;
        fpsCounter.ChangeWorking(fpsShowing);
    }
}
