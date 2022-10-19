using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControllSettingsManager : MonoBehaviour
{
    public Animator canvas;

    public static event LanguageChangeHandler OnButtonChange;
    public delegate void LanguageChangeHandler();

    public ControllButton changingButton;
    public bool waitingForKey;
    public Event keyEvent;
    public KeyCode newKey;

    [Header("Дефолтные кнопки")]
    [SerializeField] private KeyCode leftDefault;
    [SerializeField] private KeyCode rightDefault;
    [SerializeField] private KeyCode jumpDefault;
    [SerializeField] private KeyCode leftPortalDefault;
    [SerializeField] private KeyCode rightPortalDefault;
    [SerializeField] private KeyCode dialogueDefault;
    [SerializeField] private KeyCode restartDefault;

    [Header("Текст кнопок управления")]
    public ControllButton leftButton;
    public ControllButton rightButton;
    public ControllButton jumpButton;
    public ControllButton portalLeftButton;
    public ControllButton portalRightButton;
    public ControllButton dialogueButton;
    public ControllButton restartButton;

    [Header("Кнопка влево")]
    public KeyCode leftOriginall;
    public bool isLeftChanged;
    [Header("Кнопка вправо")]
    public KeyCode rightOriginall;
    public bool isRightChanged;
    [Header("Кнопка прыжка")]
    public KeyCode jumpOriginall;
    public bool isJumpChanged;
    [Header("Кнопка левого портала")]
    public KeyCode leftPortalOriginall;
    public bool isLeftPortalChanged;
    [Header("Кнопка правого портала")]
    public KeyCode rightPortalOriginall;
    public bool isRightPortalChanged;
    [Header("Кнопка диалога")]
    public KeyCode dialogueOriginall;
    public bool isDialogueChanged;
    [Header("Кнопка рестарта")]
    public KeyCode restartOriginall;
    public bool isRestartChanged;

    private void Start()
    {
        leftButton.SetText(Save.save.leftKey.ToString());
        rightButton.SetText(Save.save.rightKey.ToString());
        jumpButton.SetText(Save.save.jumpKey.ToString());
        portalLeftButton.SetText(Save.save.portalGunLeftKey.ToString());
        portalRightButton.SetText(Save.save.portalGunRightKey.ToString());
        dialogueButton.SetText(Save.save.dialogueStartKey.ToString());
        restartButton.SetText(Save.save.fastRestartKey.ToString());
        SetNewOriginall();
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
        var button = changingButton.GetComponent<Button>();
        ColorBlock cb = button.colors;
        cb.normalColor = new Color(1f, 1f, 1f, 1f);
        button.colors = cb;

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

    public IEnumerator AssignKey(string keyName)
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
        if (changingButton != leftButton)
            leftButton.GetComponent<Button>().interactable = value;
        if (changingButton != rightButton)
            rightButton.GetComponent<Button>().interactable = value;
        if (changingButton != jumpButton)
            jumpButton.GetComponent<Button>().interactable = value;
        if (changingButton != portalLeftButton)
            portalLeftButton.GetComponent<Button>().interactable = value;
        if (changingButton != portalRightButton)
            portalRightButton.GetComponent<Button>().interactable = value;
        if (changingButton != dialogueButton)
            dialogueButton.GetComponent<Button>().interactable = value;
        if (changingButton != restartButton)
            restartButton.GetComponent<Button>().interactable = value;
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