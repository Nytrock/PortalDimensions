using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    private TextAsset textFile;
    [SerializeField]
    private GameObject JustText;
    [SerializeField]
    private GameObject TextWithChoice;
    [SerializeField]
    private GameObject JustChoice;

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
    private TextMeshProUGUI MainText;
    public GameObject ChoiceButtonPrefab;
    private List<GameObject> viewChoices;

    private static Dictionary<string, Dictionary<string, string>> dialogues;
    private static Dictionary<string, Dictionary<string, string>> choices;
    private void Awake()
    {
        if (dialogues == null)
            LoadDialogues();
        SetNotActive();
        MainText = null;
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && !isButton)
        {
            if (isTextShow) {
                StopCoroutine(ShowingText());
                MainText.text = NeedText;
                isTextShow = false;
            } else if (!isAnimation) {
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
        numPanel = 1;
        var information = dialogues[dialogeKey + "_" + numPanel];
        speedShowingText = float.Parse(information["speed"], CultureInfo.InvariantCulture.NumberFormat);

        var profile = save.DialogueProfiles[int.Parse(information["id_user"])];
        Name.GetComponent<LocalizedText>().Localize(profile.name);

        switch (information["mood"]) {
            case "Calm": ProfileImage.sprite = profile.CalmImage; break;
            case "Angry": ProfileImage.sprite = profile.AngryImage; break;
            case "Afraid": ProfileImage.sprite = profile.AfraidImage; break;
            case "Happy": ProfileImage.sprite = profile.HappyImage; break;
            case "Confused": ProfileImage.sprite = profile.ConfusedImage; break;
            case "Tense": ProfileImage.sprite = profile.TenseImage; break;
        }

        if (choices.ContainsKey(dialogeKey + "_" + numPanel) && information.ContainsKey("text")) {
            TextWithChoice.SetActive(true);

            MainText = TextWithChoice.GetComponentInChildren<TextMeshProUGUI>();
            NeedText = LocalizationManager.GetTranslate(dialogeKey + "_" + numPanel);

            var choiceInformation = choices[dialogeKey + "_" + numPanel];
            foreach (var variant in choiceInformation)
            {
                var buttonChoice = Instantiate(ChoiceButtonPrefab, TextWithChoice.GetComponentInChildren<VerticalLayoutGroup>().transform);
                buttonChoice.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationManager.GetTranslate(variant.Key);
                buttonChoice.GetComponent<DialogueChoice>().NextPanel = int.Parse(variant.Value);
                buttonChoice.GetComponent<DialogueChoice>().dialogueManager = this;
                buttonChoice.SetActive(false);
                viewChoices.Add(buttonChoice);
            }
        } else if (choices.ContainsKey(dialogeKey + "_" + numPanel)) {
            JustChoice.SetActive(true);

            var choiceInformation = choices[dialogeKey + "_" + numPanel];
            foreach (var variant in choiceInformation) {
                var buttonChoice = Instantiate(ChoiceButtonPrefab, JustChoice.transform);
                buttonChoice.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationManager.GetTranslate(variant.Key);
                buttonChoice.GetComponent<DialogueChoice>().NextPanel = int.Parse(variant.Value);
                buttonChoice.GetComponent<DialogueChoice>().dialogueManager = this;
                buttonChoice.SetActive(false);
                viewChoices.Add(buttonChoice);
            }
        } else {
            JustText.SetActive(true);

            MainText = JustText.GetComponentInChildren<TextMeshProUGUI>();
            NeedText = LocalizationManager.GetTranslate(dialogeKey + "_" + numPanel);
            if (information.ContainsKey("next"))
                numPanel = int.Parse(information["next"]);
            else
                numPanel = -1;
        }

        GameObject.Find("Player").GetComponent<Player>().Dialogue();
        canvas.SetBool("isDialogue", true);
        isAnimation = true;
    }

    public void ContinueDialogue()
    {
        if (numPanel == -1) {
            canvas.SetBool("isDialogue", false);
            var player = GameObject.Find("Player").GetComponent<Player>();
            player.enabled = true;
            player.Animations.portalGun.enabled = true;
            viewChoices.Clear();
            numPanel = 1;
            return;
        }

        var information = dialogues[dialogeKey + "_" + numPanel];
        speedShowingText = float.Parse(information["speed"], CultureInfo.InvariantCulture.NumberFormat);

        var profile = save.DialogueProfiles[int.Parse(information["id_user"])];
        Name.GetComponent<LocalizedText>().Localize(profile.name);

        switch (information["mood"]) {
            case "Calm": ProfileImage.sprite = profile.CalmImage; break;
            case "Angry": ProfileImage.sprite = profile.AngryImage; break;
            case "Afraid": ProfileImage.sprite = profile.AfraidImage; break;
            case "Happy": ProfileImage.sprite = profile.HappyImage; break;
            case "Confused": ProfileImage.sprite = profile.ConfusedImage; break;
            case "Tense": ProfileImage.sprite = profile.TenseImage; break;
        }

        if (JustChoice.activeSelf) {
            isButton = false;
            viewChoices.Clear();
            for (int i = 0; i < JustChoice.transform.childCount; i++) {
                Transform child = JustChoice.transform.GetChild(i);
                Destroy(child.gameObject);
            }
        }

        if (TextWithChoice.activeSelf) {
            isButton = false;
            viewChoices.Clear();
            for (int i = 0; i < TextWithChoice.GetComponentInChildren<VerticalLayoutGroup>().transform.childCount; i++)
            {
                Transform child = TextWithChoice.GetComponentInChildren<VerticalLayoutGroup>().transform.GetChild(i);
                Destroy(child.gameObject);
            }
        }

        SetNotActive();
        if (MainText != null) {
            MainText.text = "";
            MainText = null;
        }

        if (choices.ContainsKey(dialogeKey + "_" + numPanel) && information.ContainsKey("text")) {
            TextWithChoice.SetActive(true);

            MainText = TextWithChoice.GetComponentInChildren<TextMeshProUGUI>();
            NeedText = LocalizationManager.GetTranslate(dialogeKey + "_" + numPanel);

            var choiceInformation = choices[dialogeKey + "_" + numPanel];
            foreach (var variant in choiceInformation)
            {
                var buttonChoice = Instantiate(ChoiceButtonPrefab, TextWithChoice.GetComponentInChildren<VerticalLayoutGroup>().transform);
                buttonChoice.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationManager.GetTranslate(variant.Key);
                buttonChoice.GetComponent<DialogueChoice>().NextPanel = int.Parse(variant.Value);
                buttonChoice.GetComponent<DialogueChoice>().dialogueManager = this;
                buttonChoice.SetActive(false);
                viewChoices.Add(buttonChoice);
            }
            MainText.text = "";
        } else if (choices.ContainsKey(dialogeKey + "_" + numPanel)) {
           JustChoice.SetActive(true);
            var choiceInformation = choices[dialogeKey + "_" + numPanel];
            foreach (var variant in choiceInformation) {
                var buttonChoice = Instantiate(ChoiceButtonPrefab, JustChoice.transform);
                buttonChoice.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationManager.GetTranslate(variant.Key);
                buttonChoice.GetComponent<DialogueChoice>().NextPanel = int.Parse(variant.Value);
                buttonChoice.GetComponent<DialogueChoice>().dialogueManager = this;
                buttonChoice.SetActive(false);
                viewChoices.Add(buttonChoice);
            }
        } else {
            JustText.SetActive(true);
            MainText = JustText.GetComponentInChildren<TextMeshProUGUI>();
            NeedText = LocalizationManager.GetTranslate(dialogeKey + "_" + numPanel);
            MainText.text = "";
            if (information.ContainsKey("next"))
                numPanel = int.Parse(information["next"]);
            else
                numPanel = -1;
        }
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
        if (MainText != null) {
            while (MainText.text != NeedText)
            {
                MainText.text = NeedText.Substring(0, position);
                position += 1;
                yield return new WaitForSeconds(speedShowingText);
            }
        }
        if (viewChoices.Count > 0)
        {
            foreach(GameObject button in viewChoices)
            {
                yield return new WaitForSeconds(speedShowingText * 3);
                button.SetActive(true);
            }
        }
        isTextShow = false;
    }

    public void ClearText()
    {
        SetNotActive();
        MainText.text = "";
        MainText = null;
    }

    private void LoadDialogues()
    {
        dialogues = new Dictionary<string, Dictionary<string, string>>();
        choices = new Dictionary<string, Dictionary<string, string>>();
        viewChoices = new List<GameObject>();

        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(textFile.text);

        foreach (XmlNode key in xmlDocument["Keys"].ChildNodes)
        {
            string keyStr = key.Attributes["Name"].Value;

            var values = new Dictionary<string, string>();
            foreach (XmlNode value in key.ChildNodes) {
                if (value.Name != "choice") {
                    values[value.Name] = value.InnerText;
                } else {
                    var choiceKey = keyStr;
                    var choiceValues = new Dictionary<string, string>();
                    foreach (XmlNode choice in value.ChildNodes)
                        choiceValues[choice.InnerText] = choice.Attributes["Next"].Value;
                    choices[choiceKey] = choiceValues;
                }
            }
            dialogues[keyStr] = values;
        }
    }

    private void SetNotActive()
    {
        JustText.SetActive(false);
        JustChoice.SetActive(false);
        TextWithChoice.SetActive(false);
    }

    public void ChangePanel(int newNum)
    {
        numPanel = newNum;
        ContinueDialogue();
    }
}
