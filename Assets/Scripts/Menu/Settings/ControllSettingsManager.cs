using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControllSettingsManager : MonoBehaviour
{
    public bool lightVersion;
    [SerializeField] private Animator canvas;

    public static event LanguageChangeHandler OnButtonChange;
    public delegate void LanguageChangeHandler();

    private ControllButton changingButton;
    private bool waitingForKey;
    private Event keyEvent;
    private KeyCode newKey;

    [Header("Дефолтные кнопки")]
    [SerializeField] private KeyCode leftDefault;
    [SerializeField] private KeyCode rightDefault;
    [SerializeField] private KeyCode jumpDefault;
    [SerializeField] private KeyCode leftPortalDefault;
    [SerializeField] private KeyCode rightPortalDefault;
    [SerializeField] private KeyCode dialogueDefault;
    [SerializeField] private KeyCode restartDefault;

    [Header("Текст кнопок управления")]
    [SerializeField] private ControllButton leftButton;
    [SerializeField] private ControllButton rightButton;
    [SerializeField] private ControllButton jumpButton;
    [SerializeField] private ControllButton portalLeftButton;
    [SerializeField] private ControllButton portalRightButton;
    [SerializeField] private ControllButton dialogueButton;
    [SerializeField] private ControllButton restartButton;

    [Header("Кнопки меню")]
    [SerializeField] private Button[] menuButtons;

    [Header("Кнопка влево")]
    private KeyCode leftOriginall;
    private bool isLeftChanged;
    [Header("Кнопка вправо")]
    private KeyCode rightOriginall;
    private bool isRightChanged;
    [Header("Кнопка прыжка")]
    private KeyCode jumpOriginall;
    private bool isJumpChanged;
    [Header("Кнопка левого портала")]
    private KeyCode leftPortalOriginall;
    private bool isLeftPortalChanged;
    [Header("Кнопка правого портала")]
    private KeyCode rightPortalOriginall;
    private bool isRightPortalChanged;
    [Header("Кнопка диалога")]
    private KeyCode dialogueOriginall;
    private bool isDialogueChanged;
    [Header("Кнопка рестарта")]
    private KeyCode restartOriginall;
    private bool isRestartChanged;

    private void Start()
    {
        if (!lightVersion) {
            leftButton.SetText(Save.save.leftKey.ToString());
            rightButton.SetText(Save.save.rightKey.ToString());
            jumpButton.SetText(Save.save.jumpKey.ToString());
            portalLeftButton.SetText(Save.save.portalGunLeftKey.ToString());
            portalRightButton.SetText(Save.save.portalGunRightKey.ToString());
            dialogueButton.SetText(Save.save.dialogueStartKey.ToString());
            restartButton.SetText(Save.save.fastRestartKey.ToString());
            SetNewOriginall();
        }
    }
    public void CheckChanges()
    {
        var buttonFunc = canvas.GetComponent<ButtonFunctional>();
        if (isLeftChanged || isRightChanged || isJumpChanged || isLeftPortalChanged || isRightPortalChanged || isDialogueChanged || isRestartChanged) {
            buttonFunc.SetConfirmPanel("ControllSettings");
        } else {
            buttonFunc.ControllSettings();
        }
    }

    public void StartAssignment(string keyName)
    {
        if (!waitingForKey)
            StartCoroutine(AssignKey(keyName));
    }

    IEnumerator WaitForKey()
    {
        changingButton.leftArrow.gameObject.SetActive(true);
        changingButton.rightArrow.gameObject.SetActive(true);

        while (waitingForKey)
        {
            System.Array allKeyTypes = System.Enum.GetValues(typeof(KeyCode));
            foreach (object nowKeyCode in allKeyTypes)
            {
                if (Input.GetKeyDown((KeyCode)nowKeyCode))
                {
                    newKey = (KeyCode)nowKeyCode;
                    waitingForKey = false;
                }
            }
            yield return null;
        }
    }

    IEnumerator AssignKey(string keyName)
    {
        waitingForKey = true;

        yield return WaitForKey();

        if (newKey != KeyCode.Escape)
        {
            switch (keyName)
            {
                case "Left":
                    CheckNewKey(Save.save.leftKey);
                    leftButton.SetText(newKey.ToString());
                    Save.save.leftKey = newKey;
                    isLeftChanged = leftOriginall != newKey;
                    break;
                case "Right":
                    CheckNewKey(Save.save.rightKey);
                    rightButton.SetText(newKey.ToString());
                    Save.save.rightKey = newKey;
                    isRightChanged = rightOriginall != newKey;
                    break;
                case "Jump":
                    CheckNewKey(Save.save.jumpKey);
                    jumpButton.SetText(newKey.ToString());
                    Save.save.jumpKey = newKey;
                    isJumpChanged = jumpOriginall != newKey;
                    break;
                case "LeftPortal":
                    CheckNewKey(Save.save.portalGunLeftKey);
                    portalLeftButton.SetText(newKey.ToString());
                    Save.save.portalGunLeftKey = newKey;
                    isLeftPortalChanged = leftPortalOriginall != newKey;
                    break;
                case "RightPortal":
                    CheckNewKey(Save.save.portalGunRightKey);
                    portalRightButton.SetText(newKey.ToString());
                    Save.save.portalGunRightKey = newKey;
                    isRightPortalChanged = rightPortalOriginall != newKey;
                    break;
                case "Dialogue":
                    CheckNewKey(Save.save.dialogueStartKey);
                    dialogueButton.SetText(newKey.ToString());
                    Save.save.dialogueStartKey = newKey;
                    isDialogueChanged = dialogueOriginall != newKey;
                    break;
                case "Restart":
                    CheckNewKey(Save.save.fastRestartKey);
                    restartButton.SetText(newKey.ToString());
                    Save.save.fastRestartKey = newKey;
                    isRestartChanged = restartOriginall != newKey;
                    break;
            }
        }

        changingButton = null;
        yield return new WaitForSecondsRealtime(0.1f);
        SetInteractable(true);

        yield return null;
    }
    public void SendButton(ControllButton controllButton)
    {
        changingButton = controllButton;
        SetInteractable(false);
    }

    public void ConfirmCancel(bool value)
    {
        if (value)
            ReturnToNormal();
        canvas.SetBool("isConfirm", false);
    }

    private void ReturnToNormal()
    {
        Save.save.leftKey = leftOriginall;
        Save.save.rightKey = rightOriginall;
        Save.save.jumpKey = jumpOriginall;
        Save.save.portalGunLeftKey = leftPortalOriginall;
        Save.save.portalGunRightKey = rightPortalOriginall;
        Save.save.dialogueStartKey = dialogueOriginall;
        Save.save.fastRestartKey = restartOriginall;

        leftButton.SetText(Save.save.leftKey.ToString());
        rightButton.SetText(Save.save.rightKey.ToString());
        jumpButton.SetText(Save.save.jumpKey.ToString());
        portalLeftButton.SetText(Save.save.portalGunLeftKey.ToString());
        portalRightButton.SetText(Save.save.portalGunRightKey.ToString());
        dialogueButton.SetText(Save.save.dialogueStartKey.ToString());
        restartButton.SetText(Save.save.fastRestartKey.ToString());
    }

    private void SetInteractable(bool value)
    {
        leftButton.SetButtonActive(value);
        rightButton.SetButtonActive(value);
        jumpButton.SetButtonActive(value);
        portalLeftButton.SetButtonActive(value);
        portalRightButton.SetButtonActive(value);
        dialogueButton.SetButtonActive(value);
        restartButton.SetButtonActive(value);

        foreach (Button button in menuButtons) {
            button.interactable = value;
            button.GetComponent<CellButton>().enabled = value;
        }
    }

    private void CheckNewKey(KeyCode changingKey)
    {
        if (newKey == Save.save.leftKey) {
            leftButton.SetText(changingKey.ToString());
            Save.save.leftKey = changingKey;
            isLeftChanged = leftOriginall != changingKey;
        } else if (newKey == Save.save.rightKey) {
            rightButton.SetText(changingKey.ToString());
            Save.save.rightKey = changingKey;
            isRightChanged = rightOriginall != changingKey;
        } else if (newKey == Save.save.jumpKey) {
            jumpButton.SetText(changingKey.ToString());
            Save.save.jumpKey = changingKey;
            isJumpChanged = jumpOriginall != changingKey;
        } else if (newKey == Save.save.portalGunLeftKey) {
            portalLeftButton.SetText(changingKey.ToString());
            Save.save.portalGunLeftKey = changingKey;
            isLeftPortalChanged = leftPortalOriginall != changingKey;
        } else if (newKey == Save.save.portalGunRightKey) {
            portalRightButton.SetText(changingKey.ToString());
            Save.save.portalGunRightKey = changingKey;
            isRightPortalChanged = rightPortalOriginall != changingKey;
        } else if (newKey == Save.save.dialogueStartKey) {
            dialogueButton.SetText(changingKey.ToString());
            Save.save.dialogueStartKey = changingKey;
            isDialogueChanged = dialogueOriginall != changingKey;
        } else if (newKey == Save.save.fastRestartKey) {
            restartButton.SetText(changingKey.ToString());
            Save.save.fastRestartKey = changingKey;
            isRestartChanged = restartOriginall != changingKey;
        }
    }

    public void SetNewOriginall()
    {
        leftOriginall = Save.save.leftKey;
        rightOriginall = Save.save.rightKey;
        jumpOriginall = Save.save.jumpKey;
        leftPortalOriginall = Save.save.portalGunLeftKey;
        rightPortalOriginall = Save.save.portalGunRightKey;
        dialogueOriginall = Save.save.dialogueStartKey;
        restartOriginall = Save.save.fastRestartKey;
    }

    public void SetChangesFalse()
    {
        isDialogueChanged = false;
        isJumpChanged = false;
        isLeftChanged = false;
        isLeftPortalChanged = false;
        isRestartChanged = false;
        isRightChanged = false;
        isRightPortalChanged = false;
    }

    public void SetDefaults()
    {
        leftButton.SetText(leftDefault.ToString());
        Save.save.leftKey = leftDefault;
        isLeftChanged = leftOriginall != leftDefault;

        rightButton.SetText(rightDefault.ToString());
        Save.save.rightKey = rightDefault;
        isRightChanged = rightOriginall != rightDefault;

        jumpButton.SetText(jumpDefault.ToString());
        Save.save.jumpKey = jumpDefault;
        isJumpChanged = jumpOriginall != jumpDefault;

        portalLeftButton.SetText(leftPortalDefault.ToString());
        Save.save.portalGunLeftKey = leftPortalDefault;
        isLeftPortalChanged = leftPortalOriginall != leftPortalDefault;

        portalRightButton.SetText(rightPortalDefault.ToString());
        Save.save.portalGunRightKey = rightPortalDefault;
        isRightPortalChanged = rightPortalOriginall != rightPortalDefault;

        dialogueButton.SetText(dialogueDefault.ToString());
        Save.save.dialogueStartKey = dialogueDefault;
        isDialogueChanged = dialogueOriginall != dialogueDefault;

        restartButton.SetText(restartDefault.ToString());
        Save.save.fastRestartKey = restartDefault;
        isRestartChanged = restartOriginall != restartDefault;
    }

    public void EventActive()
    {
        OnButtonChange?.Invoke();
    }
}