using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameSettingsManager : MonoBehaviour
{
    public bool lightVersion;
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
    [SerializeField] private bool defaultGlitch;

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
    [SerializeField] private Transform moreShaderVisual;
    [SerializeField] private Transform moreShaderButton;
    [SerializeField] private TextMeshProUGUI moreShaderText;

    private float moreAutoVisualPos;
    private float moreFpsVisualPos;
    private float moreExitVisualPos;
    private float moreShaderVisualPos;

    [Header("Настройки локализации")]
    private int originallyLanguage;
    private bool isLanguageChange;

    [Header("Настройки курсора")]
    public List<Texture2D> cursorsTextures;
    public RawImage cursorImage;
    public int cursorId;
    private bool isCursorChange;
    private int originalCursor;

    [Header("Настройки авторестарта")]
    public Toggle autoManager;
    private bool autorestart;
    private bool originallyAuto;
    private bool isAutoChange;

    [Header("Настройки счётчика fps")]
    public Toggle fpsManager;
    public FpsCounter fpsCounter;
    private bool fpsShowing;
    private bool originallyFps;
    private bool isFpsChange;

    [Header("Настройки подтверждения выхода")]
    public Toggle confirmManager;
    private bool confirm;
    private bool originallyConfirm;
    private bool isConfirmChange;

    [Header("Настройки шейдера")]
    public Material glitchMaterial;
    public Material noGlitchMaterial;
    public List<RawImage> quads;
    public Toggle glitchManager;
    private bool glitch;
    private bool originallyGlitch;
    private bool isGlitchChange;

    [Header("Настройки сброса прогресса")]
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI sentenceText;
    public List<string> sentences;
    public List<string> headers;
    public List<Image> buttonBlurs;
    public Button buttonReset;
    public GameObject startingResetVisual;
    public GameObject startingResetWorking;
    public GameObject endingResetVisual;
    public GameObject endingResetWorking;
    private int resetId;

    [Header("Настройки подробной информации")]
    public string toggleName;
    public TextMeshProUGUI moreHeader;
    public TextMeshProUGUI moreText;
    public Toggle moreToggle;


    public void Start()
    {
        if (!lightVersion) {
            SetNewOriginall();

            autorestart = originallyAuto;
            confirm = originallyConfirm;
            fpsShowing = originallyFps;
            glitch = originallyGlitch;

            ChangeFps();
            ChangeGlitch();
            ChangeAutorestart();
            ChangeConfirmNeed();
            SetResetBlur();
            SetChangesFalse();
            SetMorePositionsValues();
            StartCoroutine(StartButtonMorePosition());
        }
        SetShaderActive();
        ChangeCursorTexture();
        fpsCounter.ChangeWorking(fpsShowing);
    }
    public void NextLanguage()
    {
        int newId = 0;
        if (LocalizationManager.SelectedLanguage + 1 < Save.NumberLanguages)
            newId = LocalizationManager.SelectedLanguage + 1;
        LocalizationManager.localizationManager.SetLanguage(newId);
        isLanguageChange = originallyLanguage != newId;
        SetResetBlur();
        SetMoreButtonsPosition();
    }

    public void PreviousLanguage()
    {
        int newId = Save.NumberLanguages - 1;
        if (LocalizationManager.SelectedLanguage - 1 >= 0)
            newId = LocalizationManager.SelectedLanguage - 1;
        LocalizationManager.localizationManager.SetLanguage(newId);
        isLanguageChange = originallyLanguage != newId;
        SetResetBlur();
        SetMoreButtonsPosition();
    }

    public void ChangeAutorestart()
    {
        autorestart = autoManager.isOn;
        isAutoChange = autorestart != originallyAuto;
    }

    public void ChangeConfirmNeed()
    {
        confirm = confirmManager.isOn;
        isConfirmChange = confirm != originallyConfirm;
    }
    public void ChangeFps()
    {
        fpsShowing = fpsManager.isOn;
        isFpsChange = fpsShowing != originallyFps;
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

    public void ChangeGlitch()
    {
        glitch = glitchManager.isOn;
        isGlitchChange = originallyGlitch != glitch;
        SetShaderActive();
    }

    private void ChangeCursorTexture()
    {
        Cursor.SetCursor(cursorsTextures[cursorId], Vector2.zero, CursorMode.ForceSoftware);
        isCursorChange = cursorId != originalCursor;
        if (!lightVersion)
            cursorImage.texture = cursorsTextures[cursorId];
    }

    public void CheckChanges()
    {
        var buttonFunc = canvas.GetComponent<ButtonFunctional>();
        if (isLanguageChange || isAutoChange || isFpsChange || isConfirmChange || isCursorChange || isGlitchChange) {
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

        LocalizationManager.localizationManager.SetLanguage(originallyLanguage);

        cursorId = originalCursor;
        ChangeCursorTexture();

        fpsManager.isOn = originallyFps;
        fpsCounter.ChangeWorking(originallyFps);

        autoManager.isOn = originallyAuto;

        confirmManager.isOn = originallyConfirm;

        glitchManager.isOn = originallyGlitch;
        SetShaderActive();
    }

    public void SetNewOriginall()
    {
        originallyAuto = autoManager.isOn;
        originallyConfirm = confirmManager.isOn;
        originallyFps = fpsManager.isOn;
        originallyLanguage = LocalizationManager.SelectedLanguage;
        originalCursor = cursorId;
        originallyGlitch = glitchManager.isOn;
    }

    public void SetChangesFalse()
    {
        isAutoChange = false;
        isConfirmChange = false;
        isFpsChange = false;
        isCursorChange = false;
        isGlitchChange = false;
    }

    public void SetDefaults()
    {
        LocalizationManager.localizationManager.SetLanguage(defaultLanguage);
        isLanguageChange = originallyLanguage != LocalizationManager.SelectedLanguage;
        SetMoreButtonsPosition();

        cursorId = defaultCursor;
        ChangeCursorTexture();

        autoManager.isOn = defaultAuto;
        confirmManager.isOn = defaultConfirm;
        fpsManager.isOn = defaultFps;
        glitchManager.isOn = defaultGlitch;
    }

    private void SetMorePositionsValues()
    {
        moreAutoVisualPos = moreAutoVisual.localPosition.x;
        moreFpsVisualPos = moreFpsVisual.localPosition.x;
        moreExitVisualPos = moreExitVisual.localPosition.x;
        moreShaderVisualPos = moreShaderVisual.localPosition.x;
    }

    IEnumerator StartButtonMorePosition()
    {
        yield return new WaitForSeconds(0.01f);
        moreAutoVisual.localPosition = new Vector2(moreAutoVisualPos + moreAutoText.preferredWidth, moreAutoVisual.localPosition.y);
        moreFpsVisual.localPosition = new Vector2(moreFpsVisualPos + moreFpsText.preferredWidth, moreFpsVisual.localPosition.y);
        moreExitVisual.localPosition = new Vector2(moreExitVisualPos + moreExitText.preferredWidth, moreExitVisual.localPosition.y);
        moreShaderVisual.localPosition = new Vector2(moreShaderVisualPos + moreShaderText.preferredWidth, moreShaderVisual.localPosition.y);
        StartCoroutine(SetButtonMoreGlobalPositions());
    }

    private void SetMoreButtonsPosition()
    {
        moreAutoVisual.localPosition = new Vector2(moreAutoVisualPos + moreAutoText.preferredWidth, moreAutoVisual.localPosition.y);
        moreFpsVisual.localPosition = new Vector2(moreFpsVisualPos + moreFpsText.preferredWidth, moreFpsVisual.localPosition.y);
        moreExitVisual.localPosition = new Vector2(moreExitVisualPos + moreExitText.preferredWidth, moreExitVisual.localPosition.y);
        moreShaderVisual.localPosition = new Vector2(moreShaderVisualPos + moreShaderText.preferredWidth, moreShaderVisual.localPosition.y);
        StartCoroutine(SetButtonMoreGlobalPositions());
    }

    public void SetScrollBar(float value)
    {
        cursorScrollBar.value = value;
        visualScrollBar.value = value;
    }

    IEnumerator SetButtonMoreGlobalPositions()
    {
        yield return new WaitForSeconds(0.01f);
        var offset = 0f;
        if (Save.save.dialogueChoiceManager != null)
            offset = 0.15f;

        moreAutoButton.position = new Vector2(moreAutoVisual.position.x - offset, moreAutoVisual.position.y + offset);
        moreAutoButton.localPosition = new Vector3(moreAutoButton.localPosition.x, moreAutoButton.localPosition.y, 0f);

        moreFpsButton.position = new Vector2(moreFpsVisual.position.x - offset, moreFpsVisual.position.y + offset);
        moreFpsButton.localPosition = new Vector3(moreFpsButton.localPosition.x, moreFpsButton.localPosition.y, 0f);

        moreExitButton.position = new Vector2(moreExitVisual.position.x - offset, moreExitVisual.position.y + offset);
        moreExitButton.localPosition = new Vector3(moreExitButton.localPosition.x, moreExitButton.localPosition.y, 0f);

        moreShaderButton.position = new Vector2(moreShaderVisual.position.x - offset, moreShaderVisual.position.y + offset);
        moreShaderButton.localPosition = new Vector3(moreShaderButton.localPosition.x, moreShaderButton.localPosition.y, 0f);
    }

    public void StartReset()
    {
        resetId = 0;
        headerText.text = LocalizationManager.GetTranslate(headers[resetId]);
        sentenceText.text = LocalizationManager.GetTranslate(sentences[resetId]);
        SetResetAnimation();

        startingResetVisual.SetActive(true);
        startingResetWorking.SetActive(true);
        endingResetWorking.SetActive(false);
        endingResetVisual.SetActive(false);
    }

    public void ContinueReset()
    {
        resetId += 1;
        headerText.text = LocalizationManager.GetTranslate(headers[resetId]);
        sentenceText.text = LocalizationManager.GetTranslate(sentences[resetId]);

        if (resetId == sentences.Count - 1) {
            startingResetVisual.SetActive(false);
            startingResetWorking.SetActive(false);
            endingResetWorking.SetActive(true);
            endingResetVisual.SetActive(true);
            Save.save.ResetGame();
        }
    }

    public void SetResetAnimation()
    {
        canvas.SetBool("isGameReset", !canvas.GetBool("isGameReset"));
    }
    public void SetMoreAnimation()
    {
        canvas.SetBool("isMore", !canvas.GetBool("isMore"));
        toggleName = "";
    }

    public void StartMore(string toggle)
    {
        SetMoreAnimation();
        switch (toggle)
        {
            case "Autorestart": moreToggle.isOn = autoManager.isOn; break;
            case "Fps": moreToggle.isOn = fpsManager.isOn; break;
            case "ExitConfirmation": moreToggle.isOn = confirmManager.isOn; break;
            case "Shader": moreToggle.isOn = glitchManager.isOn; break;
        }
        toggleName = toggle;
        moreHeader.text = LocalizationManager.GetTranslate(toggle + "MoreHeader");
        moreText.text = LocalizationManager.GetTranslate(toggle + "MoreText");
    }

    public void ChangeToggleInMore()
    {
        switch (toggleName)
        {
            case "Autorestart": autoManager.isOn = moreToggle.isOn; break;
            case "Fps": fpsManager.isOn = moreToggle.isOn; break;
            case "ExitConfirmation": confirmManager.isOn = moreToggle.isOn; break;
            case "Shader": glitchManager.isOn = moreToggle.isOn; break;
        }
    }

    private void SetResetBlur()
    {
        if (Save.save.dialogueChoiceManager == null) {
            foreach (Image blur in buttonBlurs)
                blur.color = new Color(1f, 1f, 1f, 0);
            buttonBlurs[LocalizationManager.SelectedLanguage].color = new Color(1f, 1f, 1f, 240f / 256f);
            buttonReset.targetGraphic = buttonBlurs[LocalizationManager.SelectedLanguage];
        }
    }

    public void SetShaderActive()
    {
        if (glitch) {
            foreach (RawImage quad in quads)
                quad.material = glitchMaterial;
        } else {
            foreach (RawImage quad in quads)
                quad.material = noGlitchMaterial;
        }
    }

    public void SetFps(bool newValue)
    {
        fpsShowing = newValue;
    }

    public void SetGlitch(bool newValue)
    {
        glitch = newValue;
    }
}
