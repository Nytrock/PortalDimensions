using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Globalization;

public class DialogueManager : MonoBehaviour
{
    public Animator canvas;
    private int numPanel = 1;
    public Save save;

    public string NeedText;
    private float speedShowingText = 1f;
    private bool isTextShow;
    private bool isAnimation;
    public static bool isButton;
    private string dialogeKey;

    [Header("Статические данные")]
    public Image ProfileImage;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI MainText;

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && !isButton)
        {
            if (isTextShow) {
                StopCoroutine(ShowingText());
                MainText.text = NeedText;
                isTextShow = false;
            } else if (!isAnimation) {
                numPanel += 1;
                ContinueDialogue();
            }
        }
    }

    public void SetKey(string key)
    {
        dialogeKey = key;
    }

    public void StartDialogue()
    {
        var information = LocalizationManager.GetTranslate(dialogeKey + "_" + numPanel);
        var values = information.Split('¶');
        var profile = save.DialogueProfiles[int.Parse(values[0])];
        Name.text = profile.name;
        if (values[1] == "Calm")
            ProfileImage.sprite = profile.CalmImage;
        else if (values[1] == "Angry")
            ProfileImage.sprite = profile.AngryImage;
        speedShowingText = float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat);
        NeedText = values[3];
        GameObject.Find("Player").GetComponent<Player>().Dialogue();
        canvas.SetBool("isDialogue", true);
        isAnimation = true;
    }

    public void ContinueDialogue()
    {
        var information = LocalizationManager.GetTranslate(dialogeKey + "_" + numPanel);
        if (information == dialogeKey + "_" + numPanel) {
            canvas.SetBool("isDialogue", false);
            var player = GameObject.Find("Player").GetComponent<Player>();
            player.enabled = true;
            player.Animations.portalGun.enabled = true;
            numPanel = 1;
            return;
        }
        var values = information.Split('¶');
        var profile = save.DialogueProfiles[int.Parse(values[0])];
        Name.text = profile.name;
        if (values[1] == "Calm")
            ProfileImage.sprite = profile.CalmImage;
        else if (values[1] == "Angry")
            ProfileImage.sprite = profile.AngryImage;
        speedShowingText = float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat);
        NeedText = values[3];
        MainText.text = "";
        StartShowText();
    }

    public void StartShowText()
    {
        StartCoroutine(ShowingText());
        isTextShow = true;
        isAnimation = false;
    }

    IEnumerator ShowingText()
    {
        int position = 1;
        while (MainText.text != NeedText)
        {
            MainText.text = NeedText.Substring(0, position);
            position += 1;
            yield return new WaitForSeconds(speedShowingText);
        }
        isTextShow = false;
    }

    public void ClearText()
    {
        MainText.text = "";
    }
}
