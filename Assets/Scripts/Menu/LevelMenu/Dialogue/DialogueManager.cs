using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager dialogueManager;
    private Player player;

    [SerializeField] private TextAsset textFile;
    [SerializeField] private GameObject JustText;
    [SerializeField] private GameObject TextWithChoice;
    [SerializeField] private GameObject TextWithChoiceContainer;
    [SerializeField] private GameObject JustChoice;
    [SerializeField] private GameObject JustChoiceContainer;

    public Choice choiceArrow;
    public DialogueChoiceManager choiceManager;

    public Animator panelsController;
    private int numPanel = 1;
    public Save save;

    public string NeedText;
    private float speedShowingText = 1f;
    public bool isTextShow;
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
    private static Dictionary<string, Dictionary<string, Dictionary<string, string>>> choices;
    private void Awake()
    {
        dialogueManager = this;
        if (dialogues == null)
            LoadDialogues();
        SetNotActive();
        MainText = null;
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && !isButton && panelsController.GetBool("isDialogue") && !ButtonFunctional.isGamePaused)
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
                CheckRobotAtEnd();
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
        speedShowingText = 1 / (10 * speedShowingText) / 1.5f;

        SetProfileInformation(information);

        if (choices.ContainsKey(dialogeKey + "_" + numPanel) && information.ContainsKey("text")) {
            SetTextPanel(TextWithChoice);
            SetChoicePanel(TextWithChoice, TextWithChoice.GetComponentInChildren<VerticalLayoutGroup>().transform, TextWithChoiceContainer.transform, 1);
        } else if (choices.ContainsKey(dialogeKey + "_" + numPanel)) {
            SetChoicePanel(JustChoice, JustChoice.transform, JustChoiceContainer.transform, 0);
        } else {
            JustText.SetActive(true);
            SetTextPanel(JustText);
        }

        player.StopWorking();
        panelsController.SetBool("isDialogue", true);
        isAnimation = true;
    }

    public void ContinueDialogue()
    {
        var information = dialogues[dialogeKey + "_" + numPanel];
        if (JustText.activeSelf)
            numPanel = int.Parse(information["next"]);

        if (numPanel == -1) {
            panelsController.SetBool("isDialogue", false);
            player.enabled = true;
            player.animations.portalGun.enabled = true;
            viewChoices.Clear();
            choiceArrow.Buttons.Clear();
            numPanel = 1;
            save.SaveAll();
            return;
        } else {
            information = dialogues[dialogeKey + "_" + numPanel];
        }

        speedShowingText = float.Parse(information["speed"], CultureInfo.InvariantCulture.NumberFormat);
        speedShowingText = 1 / (10 * speedShowingText) / 1.5f;

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
        }
        StartShowText();
    }

    public void StartShowText()
    {
        StartCoroutine(ShowingText());
        isTextShow = true;
        isAnimation = false;

        var information = dialogues[dialogeKey + "_" + numPanel];
        var profile = save.DialogueProfiles[int.Parse(information["id_user"])];
        if (profile.isRobot) {
            var animator = ProfileImage.GetComponent<Animator>();
            animator.SetFloat("Speed", 1 / (speedShowingText * 10) / 1.5f);
        }
    }

    public void ChangeMainText()
    {
        if (MainText.text.Length < NeedText.Length){
            MainText.text = NeedText.Substring(0, MainText.text.Length);
        } else {
            MainText.text = NeedText;
            StopAllCoroutines();
            isTextShow = false;
        }
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

        CheckRobotAtEnd();
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
        choices = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
        viewChoices = new Dictionary<GameObject, GameObject>();
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(textFile.text);

        foreach (XmlNode key in xmlDocument["keys"].ChildNodes)
        {
            string keyStr = key.Attributes["name"].Value;

            var values = new Dictionary<string, string>();
            foreach (XmlNode value in key.ChildNodes) {
                if (value.Name != "choice") {
                    values[value.Name] = value.InnerText;
                } else {
                    var choiceKey = keyStr;
                    var choiceValues = new Dictionary<string, Dictionary<string, string>>();
                    foreach (XmlNode choice in value.ChildNodes)
                    {
                        var choice_information = new Dictionary<string, string>();
                        choice_information["next"] = choice.Attributes["next"].Value;
                        if (choice.Attributes["required"] != null)
                            choice_information["required"] = "true";
                        if (choice.Attributes["do_id"] != null)
                            choice_information["do_id"] = choice.Attributes["do_id"].Value;
                        if (choice.Attributes["existing_id"] != null)
                            choice_information["existing_id"] = choice.Attributes["existing_id"].Value;
                        choiceValues[choice.InnerText] = choice_information;
                    }
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

        ProfileImage.color = profile.profileColor;

        if (profile.isRobot) {
            var animator = ProfileImage.GetComponent<Animator>();
            animator.enabled = true;
            animator.SetFloat("Speed", 0);

            SetAllStatesFalse(animator);

            switch (information["mood"]) {
                case "Calm": animator.SetBool("isCalm", true); break;
                case "Angry": animator.SetBool("isAngry", true); break;
                case "Afraid": animator.SetBool("isAfraid", true); break;
                case "Happy": animator.SetBool("isHappy", true); break;
                case "Confused": animator.SetBool("isConfused", true); break;
                case "Tense": animator.SetBool("isTense", true); break;
            }
            animator.Play(information["mood"], 0, 0f);
        } else {
            switch (information["mood"]) {
                case "Calm": ProfileImage.sprite = profile.CalmImage; break;
                case "Angry": ProfileImage.sprite = profile.AngryImage; break;
                case "Afraid": ProfileImage.sprite = profile.AfraidImage; break;
                case "Happy": ProfileImage.sprite = profile.HappyImage; break;
                case "Confused": ProfileImage.sprite = profile.ConfusedImage; break;
                case "Tense": ProfileImage.sprite = profile.TenseImage; break;
            }
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

        choiceArrow.transform.localPosition = new Vector2(choiceArrow.transform.localPosition.x, choiceArrow.positions[num]);
        choiceArrow.TargetPosition = choiceArrow.positions[num];
        choiceArrow.NowPosition = choiceArrow.positions[num];

        var updatedChoiceInformation = new Dictionary<string, Dictionary<string, string>>();
        var lastKey = "";
        KeyValuePair<string, Dictionary<string, string>> main = new KeyValuePair<string, Dictionary<string, string>>();
        foreach (var variant in choiceInformation){ 
            if (variant.Value.ContainsKey("required")) {
                main = variant;
            } else {
                bool existing = true;
                if (variant.Value.ContainsKey("existing_id")) {
                    existing = save.GetChoiceExisting(int.Parse(variant.Value["existing_id"]));
                }
                if (updatedChoiceInformation.Count < 4 && existing) {
                    updatedChoiceInformation[variant.Key] = variant.Value;
                    lastKey = variant.Key;
                }
            }
        }
        if (main.Key != null) {
            if (updatedChoiceInformation.Count == 4)
                updatedChoiceInformation.Remove(lastKey);
            updatedChoiceInformation[main.Key] = main.Value;
        }


        foreach (var variant in updatedChoiceInformation) {
            var visualbuttonChoice = Instantiate(VisualChoiceButtonPrefab, VisualButtonsContainers);
            visualbuttonChoice.GetComponentInChildren<TextMeshProUGUI>().text = variant.Key;
            visualbuttonChoice.GetComponentInChildren<LocalizedText>().Localize();
            visualbuttonChoice.SetActive(false);

            var workingbuttonChoice = Instantiate(WorkingChoiceButtonPrefab, WorkingButtonsContainers);
            var dialogueChoice = workingbuttonChoice.GetComponent<DialogueChoice>();
            dialogueChoice.NextPanel = int.Parse(variant.Value["next"]);
            dialogueChoice.dialogueManager = this;
            dialogueChoice.id = num;
            if (variant.Value.ContainsKey("do_id"))
                dialogueChoice.doId = int.Parse(variant.Value["do_id"]);
            if(variant.Value.ContainsKey("existing_id"))
                dialogueChoice.existingId = int.Parse(variant.Value["existing_id"]);
            workingbuttonChoice.SetActive(false);

            choiceArrow.Buttons.Add(workingbuttonChoice.GetComponent<Button>());

            viewChoices[visualbuttonChoice] = workingbuttonChoice;
            num += 1;
        }
    }

    private void SetTextPanel(GameObject container) {
        MainText = container.GetComponentInChildren<TextMeshProUGUI>();
        MainText.GetComponent<LocalizedDialogueText>().key = dialogeKey + "_" + numPanel;
        NeedText = LocalizationManager.GetTranslate(dialogeKey + "_" + numPanel);
    }

    private void SetAllStatesFalse(Animator animator) {
        animator.SetBool("isCalm", false);
        animator.SetBool("isAngry", false);
        animator.SetBool("isAfraid", false);
        animator.SetBool("isHappy", false);
        animator.SetBool("isConfused", false);
        animator.SetBool("isTense", false);
    }

    private void SetImageAfterAnimation(Dictionary<string, string> information)
    {
        var profile = save.DialogueProfiles[int.Parse(information["id_user"])];

        switch (information["mood"]) {
            case "Calm": ProfileImage.sprite = profile.CalmImage; break;
            case "Angry": ProfileImage.sprite = profile.AngryImage; break;
            case "Afraid": ProfileImage.sprite = profile.AfraidImage; break;
            case "Happy": ProfileImage.sprite = profile.HappyImage; break;
            case "Confused": ProfileImage.sprite = profile.ConfusedImage; break;
            case "Tense": ProfileImage.sprite = profile.TenseImage; break;
        }
    }

    private void CheckRobotAtEnd()
    {
        var information = dialogues[dialogeKey + "_" + numPanel];
        var profile = save.DialogueProfiles[int.Parse(information["id_user"])];
        if (profile.isRobot) {
            SetAllStatesFalse(ProfileImage.GetComponent<Animator>());
            ProfileImage.GetComponent<Animator>().enabled = false;
            SetImageAfterAnimation(information);
        }
    }
    public void SetPlayer(Player newPlayer) {
        player = newPlayer;
    }
}
