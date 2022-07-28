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

    [Header("Текст кнопок управления")]
    public TextMeshProUGUI leftButtonText;
    public TextMeshProUGUI rightButtonText;
    public TextMeshProUGUI jumpButtonText;
    public TextMeshProUGUI portalLeftButtonText;
    public TextMeshProUGUI portalRightButtonText;
    public TextMeshProUGUI dialogueButtonText;
    public TextMeshProUGUI restartButtonText;

    [Header("Кнопка влево")]
    public KeyCode leftOriginall;
    public bool isLeftChanged;
    public KeyCode leftNow;

    private void Start()
    {
        leftButtonText.text = Save.save.leftKey.ToString();
        leftOriginall = Save.save.leftKey;
        leftNow = leftOriginall;
    }
    public void CheckChanges()
    {
        canvas.SetBool("isSettingsControll", !canvas.GetBool("isSettingsControll"));
    }

    private void OnGUI()
    {
        keyEvent = Event.current;

        if (keyEvent.isKey && waitingForKey)
        {
            newKey = keyEvent.keyCode;
            waitingForKey = false;
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

        while (!keyEvent.isKey)
            yield return null;
    }

    public IEnumerator AssignKey(string keyName)
    {
        waitingForKey = true;

        yield return WaitForKey();

        if (newKey != KeyCode.Escape) {
            switch (keyName) {
                case "Left":
                    leftButtonText.text = newKey.ToString();
                    leftNow = newKey;
                    isLeftChanged = leftOriginall != newKey;
                    break;
            }
        }
        

        changingButton.UpdateLeftArrowAndBlur();

		yield return null;
	}
	public void SendButton(ControllButton controllButton)
    {
        changingButton = controllButton;
    }
}
