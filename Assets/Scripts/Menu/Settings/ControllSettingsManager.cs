using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ControllSettingsManager : MonoBehaviour
{
    public Animator canvas;

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
        if (isLeftChanged || isRightChanged || isJumpChanged || isLeftPortalChanged || isRightPortalChanged || isDialogueChanged || isRestartChanged)
        {
            canvas.GetComponent<ButtonFunctional>().SetConfirmPanel("ControllSettings");
        }
        else
        {
            canvas.SetBool("isSettingsControll", !canvas.GetBool("isSettingsControll"));
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
                    leftButton.SetText(newKey.ToString());
                    Save.save.leftKey = newKey;
                    isLeftChanged = leftOriginall != newKey;
                    break;
                case "Right":
                    rightButton.SetText(newKey.ToString());
                    Save.save.rightKey = newKey;
                    isRightChanged = rightOriginall != newKey;
                    break;
                case "Jump":
                    jumpButton.SetText(newKey.ToString());
                    Save.save.jumpKey = newKey;
                    isJumpChanged = jumpOriginall != newKey;
                    break;
                case "LeftPortal":
                    portalLeftButton.SetText(newKey.ToString());
                    Save.save.portalGunLeftKey = newKey;
                    isLeftPortalChanged = leftPortalOriginall != newKey;
                    break;
                case "RightPortal":
                    portalRightButton.SetText(newKey.ToString());
                    Save.save.portalGunRightKey = newKey;
                    isRightPortalChanged = rightPortalOriginall != newKey;
                    break;
                case "Dialogue":
                    dialogueButton.SetText(newKey.ToString());
                    Save.save.dialogueStartKey = newKey;
                    isDialogueChanged = dialogueOriginall != newKey;
                    break;
                case "Restart":
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

        isLeftChanged = isRightChanged = isJumpChanged = isLeftPortalChanged = isRightPortalChanged = isDialogueChanged = isRestartChanged = false;

        canvas.SetBool("isSettingsControll", false);
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
}