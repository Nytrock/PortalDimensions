using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    private TextAsset textFile;
    [SerializeField]
    private GameObject JustText;
    [SerializeField]
    private GameObject TextWithChoice;
    [SerializeField]
    private GameObject TextWithChoiceContainer;
    [SerializeField]
    private GameObject JustChoice;
    [SerializeField]
    private GameObject JustChoiceContainer;

    public Choice choiceArrow;

    public Animator panelsController;
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
    public GameObject VisualChoiceButtonPrefab;
    public GameObject WorkingChoiceButtonPrefab;
    private Dictionary<GameObject, GameObject> viewChoices;

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
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && !isButton && panelsController.GetBool("isDialogue") && !CanvasManager.isGamePaused)
        {
            if (isTextShow) {
                StopAllCoroutines();
                if (MainText != null)
                    MainText.text = NeedText;
                foreach (var button in viewChoices) {
                    button.Key.SetActive(true);
                    button.Value.SetActive(true);
                }
                if (JustChoice.activeSelf || TextWithChoice.activeSelf)
                    choiceArrow.gameObject.SetActive(true);
                isTextShow = false;
            } else if (!isAnimation && !JustChoice.activeSelf && !TextWithChoice.activeSelf) {
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

        SetProfileInformation(information);

        if (choices.ContainsKey(dialogeKey + "_" + numPanel) && information.ContainsKey("text")) {
            SetTextPanel(TextWithChoice);
            SetChoicePanel(TextWithChoice, TextWithChoice.GetComponentInChildren<VerticalLayoutGroup>().transform, TextWithChoiceContainer.transform, 1);
        } else if (choices.ContainsKey(dialogeKey + "_" + numPanel)) {
            SetChoicePanel(JustChoice, JustChoice.transform, JustChoiceContainer.transform, 0);
        } else {
            JustText.SetActive(true);
            SetTextPanel(JustText);
            if (information.ContainsKey("next"))
                numPanel = int.Parse(information["next"]);
            else
                numPanel = -1;
        }

        GameObject.Find("Player").GetComponent<Player>().Dialogue();
        panelsController.SetBool("isDialogue", true);
        isAnimation = true;
    }

    public void ContinueDialogue()
    {
        if (numPanel == -1) {
            panelsController.SetBool("isDialogue", false);
            var player = GameObject.Find("Player").GetComponent<Player>();
            player.enabled = true;
            player.Animations.portalGun.enabled = true;
            viewChoices.Clear();
            choiceArrow.Buttons.Clear();
            numPanel = 1;
            return;
        }

        var information = dialogues[dialogeKey + "_" + numPanel];
        speedShowingText = float.Parse(information["speed"], CultureInfo.InvariantCulture.NumberFormat);

        SetProfileInformation(information);

        ClearPreviousChoices(JustChoice, JustChoiceContainer);
        ClearPreviousChoices(TextWithChoice, TextWithChoiceContainer);

        SetNotActive();
        if (MainText != null) {
            MainText.text = "";
            MainText = null;
        }

        if (choices.ContainsKey(dialogeKey + "_" + numPanel) && information.ContainsKey("text")) {
            SetTextPanel(TextWithChoice);
            SetChoicePanel(TextWithChoice, TextWithChoice.GetComponentInChildren<VerticalLayoutGroup>().transform, TextWithChoiceContainer.transform, 1);
            MainText.text = "";
        } else if (choices.ContainsKey(dialogeKey + "_" + numPanel)) {
            SetChoicePanel(JustChoice, JustChoice.transform, JustChoiceContainer.transform, 0);
        } else {
            JustText.SetActive(true);
            SetTextPanel(JustText);
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
        if (viewChoices.Count > 0) {
            yield return new WaitForSeconds(speedShowingText * 3);
            choiceArrow.gameObject.SetActive(true);
            foreach (var button in viewChoices)
            {
                button.Key.SetActive(true);
                button.Value.SetActive(true);
                yield return new WaitForSeconds(speedShowingText * 2);
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
        viewChoices = new Dictionary<GameObject, GameObject>();
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
        choiceArrow.gameObject.SetActive(false);
    }

    public void ChangePanel(int newNum)
    {
        numPanel = newNum;
        ContinueDialogue();
    }

    private void SetProfileInformation(Dictionary<string, string> information)
    {
        var profile = save.DialogueProfiles[int.Parse(information["id_user"])];
        Name.GetComponent<LocalizedText>().Localize(profile.name);

        switch (information["mood"])
        {
            case "Calm": ProfileImage.sprite = profile.CalmImage; break;
            case "Angry": ProfileImage.sprite = profile.AngryImage; break;
            case "Afraid": ProfileImage.sprite = profile.AfraidImage; break;
            case "Happy": ProfileImage.sprite = profile.HappyImage; break;
            case "Confused": ProfileImage.sprite = profile.ConfusedImage; break;
            case "Tense": ProfileImage.sprite = profile.TenseImage; break;
        }
    }

    private void ClearPreviousChoices(GameObject MainContainer, GameObject SecondContainer)
    {
        if (MainContainer.activeSelf) {
            isButton = false;
            viewChoices.Clear();
            choiceArrow.Buttons.Clear();
            for (int i = 0; i < MainContainer.transform.childCount; i++) {
                Transform child = MainContainer.transform.GetChild(i);
                Destroy(child.gameObject);
            }
            for (int i = 0; i < SecondContainer.transform.childCount; i++) {
                Transform child = SecondContainer.transform.GetChild(i);
                Destroy(child.gameObject);
            }
        }
    }

    private void SetChoicePanel(GameObject MainPanel, Transform VisualButtonsContainers, Transform WorkingButtonsContainers, int StartNum)
    {
        MainPanel.SetActive(true);

        var choiceInformation = choices[dialogeKey + "_" + numPanel];
        int num = StartNum;

        choiceArrow.transform.position = new Vector2(choiceArrow.transform.position.x, choiceArrow.positions[num]);
        choiceArrow.TargetPosition = choiceArrow.positions[num];
        choiceArrow.NowPosition = choiceArrow.positions[num];

        foreach (var variant in choiceInformation)
        {
            var visualbuttonChoice = Instantiate(VisualChoiceButtonPrefab, VisualButtonsContainers);
            visualbuttonChoice.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationManager.GetTranslate(variant.Key);
            visualbuttonChoice.SetActive(false);

            var workingbuttonChoice = Instantiate(WorkingChoiceButtonPrefab, WorkingButtonsContainers);
            workingbuttonChoice.GetComponent<DialogueChoice>().NextPanel = int.Parse(variant.Value);
            workingbuttonChoice.GetComponent<DialogueChoice>().dialogueManager = this;
            workingbuttonChoice.GetComponent<DialogueChoice>().id = num;
            workingbuttonChoice.SetActive(false);

            choiceArrow.Buttons.Add(workingbuttonChoice.GetComponent<Button>());

            viewChoices[visualbuttonChoice] = workingbuttonChoice;
            num += 1;
        }
    }

    private void SetTextPanel(GameObject container) {
        MainText = container.GetComponentInChildren<TextMeshProUGUI>();
        NeedText = LocalizationManager.GetTranslate(dialogeKey + "_" + numPanel);
    }
}
