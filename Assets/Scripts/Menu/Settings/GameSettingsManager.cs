using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameSettingsManager : MonoBehaviour
{
    public Animator canvas;

    [Header("Скроллбары")]
    public Scrollbar visualScrollBar;
    public Scrollbar cursorScrollBar;

    [Header("Дефолтные настройки")]
    [SerializeField] private int defaultLanguage;
    [SerializeField] private int defaultCursor;
    [SerializeField] private bool defaultAuto;
    [SerializeField] private bool defaultFps;
    [SerializeField] private bool defaultConfirm;

    [Header("Вкладка подробнее")]
    [SerializeField] private Transform moreAutoVisual;
    [SerializeField] private Transform moreAutoButton;
    [SerializeField] private TextMeshProUGUI moreAutoText;
    [SerializeField] private Transform moreFpsVisual;
    [SerializeField] private Transform moreFpsButton;
    [SerializeField] private TextMeshProUGUI moreFpsText;
    [SerializeField] private Transform moreExitVisual;
    [SerializeField] private Transform moreExitButton;
    [SerializeField] private TextMeshProUGUI moreExitText;

    private float moreAutoVisualPos;
    private float moreFpsVisualPos;
    private float moreExitVisualPos;

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
        SetMorePositionsValues();
        SetMoreButtonsPosition(true);

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
        SetMoreButtonsPosition(false);
    }

    public void PreviousLanguage()
    {
        int newId = Save.NumberLanguages - 1;
        if (LocalizationManager.SelectedLanguage - 1 >= 0)
            newId = LocalizationManager.SelectedLanguage - 1;
        localization.SetLanguage(newId);
        isLanguageChange = originallyLanguage != newId;
        SetMoreButtonsPosition(false);
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

    private void SetMorePositionsValues()
    {
        moreAutoVisualPos = moreAutoVisual.localPosition.x;
        moreFpsVisualPos = moreFpsVisual.localPosition.x;
        moreExitVisualPos = moreExitVisual.localPosition.x;
    }

    private void SetMoreButtonsPosition(bool start)
    {
        float addingValue = 0;
        if (start)
            addingValue = 0.12f;
        moreAutoVisual.localPosition = new Vector2(moreAutoVisualPos + moreAutoText.preferredWidth, moreAutoVisual.localPosition.y);
        moreFpsVisual.localPosition = new Vector2(moreFpsVisualPos + moreFpsText.preferredWidth, moreFpsVisual.localPosition.y);
        moreExitVisual.localPosition = new Vector2(moreExitVisualPos + moreExitText.preferredWidth, moreExitVisual.localPosition.y);
        moreAutoButton.position = new Vector2(moreAutoVisual.position.x, moreAutoVisual.position.y + addingValue);
        moreFpsButton.position = new Vector3(moreFpsVisual.position.x, moreFpsVisual.position.y + addingValue);
        moreExitButton.position = new Vector3(moreExitVisual.position.x, moreExitVisual.position.y + addingValue);
    }

    public void SetScrollBar(float value)
    {
        cursorScrollBar.value = value;
        visualScrollBar.value = value;
    }
}
