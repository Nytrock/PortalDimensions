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
    public static bool isButton;
    private PlayerStateManager player;

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
    [SerializeField] private AudioSource textShowSound;

    public string NeedText;
    public bool isTextShow;
    private bool isAnimation;
    private float speedShowingText = 1f;
    private string dialogueKey;
    [SerializeField] private int doId = -1;

    [Header("Статические данные")]
    public Image ProfileImage;
    public TextMeshProUGUI Name;
    private TextMeshProUGUI MainText;
    public GameObject VisualChoiceButtonPrefab;
    public GameObject WorkingChoiceButtonPrefab;
    private Dictionary<GameObject, GameObject> viewChoices = new();

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
                if (MainText != null) {
                    ClearTextFromCommands();
                    MainText.text = NeedText;
                }
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
        dialogueKey = key;
    }

    public void StartDialogue()
    {
        Time.fixedDeltaTime = 0.02f;
        numPanel = 0;
        var information = dialogues[dialogueKey + "_" + numPanel];
        speedShowingText = float.Parse(information["speed"], CultureInfo.InvariantCulture.NumberFormat);
        speedShowingText = 1 / (10 * speedShowingText) / 1.5f;

        SetProfileInformation(information);

        if (choices.ContainsKey(dialogueKey + "_" + numPanel) && information.ContainsKey("text")) {
            SetTextPanel(TextWithChoice);
            SetChoicePanel(TextWithChoice, TextWithChoice.GetComponentInChildren<VerticalLayoutGroup>().transform, TextWithChoiceContainer.transform, 1);
        } else if (choices.ContainsKey(dialogueKey + "_" + numPanel)) {
            SetChoicePanel(JustChoice, JustChoice.transform, JustChoiceContainer.transform, 0);
        } else {
            JustText.SetActive(true);
            SetTextPanel(JustText);
        }

        if (information.ContainsKey("do_id"))
            doId = int.Parse(information["do_id"]);

        player.SwitchState(player.disabledState);
        panelsController.SetBool("isDialogue", true);
        if (bool.Parse(information["instant"])) {
            panelsController.Play("StartDialogue", 0, 0f);
            StartCoroutine(StartShowText(0.3f));
        } else {
            StartCoroutine(StartShowText(1.3f));
        }
        isAnimation = true;
    }

    public void ContinueDialogue()
    {
        var information = dialogues[dialogueKey + "_" + numPanel];
        if (JustText.activeSelf)
            numPanel = int.Parse(information["next"]);
        if (doId != -1) {
            choiceManager.DoSomethingFromId(doId);
            save.SetChoiceDoing(doId, true);
            doId = -1;
        }

        if (numPanel == -1) {
            panelsController.SetBool("isDialogue", false);
            if (bool.Parse(information["instant"])) {
                panelsController.Play("EndDialogue", 0, 0f);
                StartCoroutine(ClearText(0.3f));
            } else {
                StartCoroutine(ClearText(1.3f));
            }
            player.SwitchState(player.calmState);
            viewChoices.Clear();
            choiceArrow.Buttons.Clear();
            numPanel = 0;
            Time.fixedDeltaTime = 0.002f;
            save.SaveAll();
            return;
        } else {
            information = dialogues[dialogueKey + "_" + numPanel];
        }

        speedShowingText = float.Parse(information["speed"], CultureInfo.InvariantCulture.NumberFormat);
        speedShowingText = 1 / (10 * speedShowingText) / 1.5f;

        if (information.ContainsKey("do_id"))
            doId = int.Parse(information["do_id"]);

        SetProfileInformation(information);

        ClearPreviousChoices(JustChoice, JustChoiceContainer);
        ClearPreviousChoices(TextWithChoice, TextWithChoiceContainer);

        SetNotActive();
        if (MainText != null) {
            MainText.text = "";
            MainText = null;
        }

        if (choices.ContainsKey(dialogueKey + "_" + numPanel) && information.ContainsKey("text")) {
            SetTextPanel(TextWithChoice);
            SetChoicePanel(TextWithChoice, TextWithChoice.GetComponentInChildren<VerticalLayoutGroup>().transform, TextWithChoiceContainer.transform, 1);
            MainText.text = "";
        } else if (choices.ContainsKey(dialogueKey + "_" + numPanel)) {
            SetChoicePanel(JustChoice, JustChoice.transform, JustChoiceContainer.transform, 0);
        } else {
            JustText.SetActive(true);
            SetTextPanel(JustText);
            MainText.text = "";
        }
        StartCoroutine(StartShowText(0f));
    }

    private IEnumerator StartShowText(float wait)
    {
        yield return new WaitForSeconds(wait);
        isTextShow = true;
        isAnimation = false;

        var information = dialogues[dialogueKey + "_" + numPanel];
        var profile = save.dialogueProfiles[int.Parse(information["id_user"])];
        var animator = ProfileImage.GetComponent<Animator>();
        if (profile.isRobot) {
            animator.SetFloat("Speed", speedShowingText * 15);
        }

        NeedText = NeedText.Replace("{GK_Left}", Save.save.leftKey.ToString());
        NeedText = NeedText.Replace("{GK_Right}", Save.save.rightKey.ToString());
        NeedText = NeedText.Replace("{GK_Jump}", Save.save.jumpKey.ToString());
        NeedText = NeedText.Replace("{GK_LeftPortal}", Save.save.portalGunLeftKey.ToString());
        NeedText = NeedText.Replace("{GK_RightPortal}", Save.save.portalGunRightKey.ToString());
        NeedText = NeedText.Replace("{GK_Act}", Save.save.dialogueStartKey.ToString());
        NeedText = NeedText.Replace("{GK_Restart}", Save.save.fastRestartKey.ToString());

        int position = 0;
        if (MainText != null) {
            while (NeedText.Length != position) {
                if (NeedText[position] == '{') {
                    int end = position + 1;
                    var command = "";
                    while (NeedText[end] != '}') {
                        command += NeedText[end];
                        end += 1;
                    }

                    if (command.Contains("WS")) {
                        SetAllStatesFalse(animator);
                        yield return new WaitForSeconds(float.Parse(command.Split('_')[1], CultureInfo.InvariantCulture.NumberFormat));
                        SetProfileInformation(information);
                        NeedText = NeedText.Remove(position, end - position + 1);
                        position -= 1;
                    } else if (command.Contains("CS")) {
                        speedShowingText = float.Parse(command.Split('_')[1], CultureInfo.InvariantCulture.NumberFormat);
                        speedShowingText = 1 / (speedShowingText * 10) / 1.5f;
                        NeedText = NeedText.Remove(position, end - position + 1);
                        position -= 1;
                    } else if (command.Contains("CE")) {
                        position -= 1;
                        information["mood"] = command.Split('_')[1];
                        SetProfileInformation(information);
                        NeedText = NeedText.Remove(position, end - position + 1);
                        position -= 1;
                    } else {
                        MainText.text += NeedText[position];
                        if (NeedText[position] != ' ')
                            textShowSound.Play();
                    }
                    animator.SetFloat("Speed", speedShowingText * 15);
                } else {
                    MainText.text += NeedText[position];
                    if (NeedText[position] != ' ')
                        textShowSound.Play();
                }
                position += 1;
                yield return new WaitForSeconds(speedShowingText);
            }
        }

        if (viewChoices.Count > 0) {
            if (animator.GetBool("isCalm"))
                ProfileImage.sprite = profile.CalmImage;
            if (animator.GetBool("isAngry"))
                ProfileImage.sprite = profile.AngryImage;
            if (animator.GetBool("isAfraid"))
                ProfileImage.sprite = profile.AfraidImage;
            if (animator.GetBool("isConfused"))
                ProfileImage.sprite = profile.ConfusedImage;
            if (animator.GetBool("isHappy"))
                ProfileImage.sprite = profile.HappyImage;
            if (animator.GetBool("isTense"))
                ProfileImage.sprite = profile.TenseImage;
            SetAllStatesFalse(animator);
            animator.enabled = false;

            yield return new WaitForSeconds(speedShowingText * 3);
            choiceArrow.gameObject.SetActive(true);
            foreach (var button in viewChoices) {
                button.Key.SetActive(true);
                button.Value.SetActive(true);
                yield return new WaitForSeconds(speedShowingText * 2);
            }
        }

        CheckRobotAtEnd();
        isTextShow = false;
    }

    public void ChangeMainText()
    {
        if (MainText.text.Length < NeedText.Length){
            var newText = NeedText.Substring(0, MainText.text.Length);
            int position = 0;
            while (newText.Length != position) {
                if (newText[position] == '{') {
                    int end = position + 1;
                    var command = "{";
                    while (newText[end] != '}') {
                        command += newText[end];
                        end += 1;
                    }
                    if (command.Contains("{CS") || command.Contains("{WS") || command.Contains("{CE"))
                        newText = newText.Remove(position, end - position + 1);
                    else
                        position += 1;
                } else {
                    position += 1;
                }
            }
            MainText.text = newText;
        } else {
            ClearTextFromCommands();
            MainText.text = NeedText;
            StopAllCoroutines();
            isTextShow = false;
        }
    }

    private IEnumerator ClearText(float wait)
    {
        yield return new WaitForSeconds(wait);
        SetNotActive();
        MainText.text = "";
    }

    private void LoadDialogues()
    {
        dialogues = new Dictionary<string, Dictionary<string, string>>();
        choices = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
        XmlDocument xmlDocument = new();
        xmlDocument.LoadXml(textFile.text);

        foreach (XmlNode key in xmlDocument["keys"].ChildNodes)
        {
            string keyStr = key.Attributes["name"].Value;
            var instant = key.Attributes["instant"];

            var values = new Dictionary<string, string>();
            foreach (XmlNode value in key.ChildNodes) {
                values["instant"] = (instant != null).ToString();
                if (value.Name != "choice") {
                    values[value.Name] = value.InnerText;
                    if (key.Attributes["do_id"] != null)
                        values["do_id"] = key.Attributes["do_id"].Value;
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
        var profile = save.dialogueProfiles[int.Parse(information["id_user"])];
        Name.GetComponent<LocalizedText>().Localize(profile.header);

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

        var choiceInformation = choices[dialogueKey + "_" + numPanel];
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
            visualbuttonChoice.GetComponentInChildren<LocalizedText>().Localize(variant.Key);
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
            workingbuttonChoice.GetComponent<ButtonSwitch>().SetAnimator(choiceArrow.GetComponent<Animator>());

            choiceArrow.Buttons.Add(workingbuttonChoice.GetComponent<Button>());

            viewChoices[visualbuttonChoice] = workingbuttonChoice;
            num += 1;
        }
    }

    private void SetTextPanel(GameObject container) {
        MainText = container.GetComponentInChildren<TextMeshProUGUI>();
        MainText.GetComponent<LocalizedDialogueText>().key = dialogueKey + "_" + numPanel;
        NeedText = LocalizationManager.GetTranslate(dialogueKey + "_" + numPanel);
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
        var profile = save.dialogueProfiles[int.Parse(information["id_user"])];

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
        var information = dialogues[dialogueKey + "_" + numPanel];
        var profile = save.dialogueProfiles[int.Parse(information["id_user"])];
        if (profile.isRobot) {
            SetAllStatesFalse(ProfileImage.GetComponent<Animator>());
            ProfileImage.GetComponent<Animator>().enabled = false;
            SetImageAfterAnimation(information);
        }
    }
    public void SetPlayer(PlayerStateManager newPlayer) {
        player = newPlayer;
    }

    private void ClearTextFromCommands()
    {
        int position = 0;
        while (NeedText.Length != position) {
            if (NeedText[position] == '{') {
                int end = position + 1;
                var command = "{";
                while (NeedText[end] != '}') {
                    command += NeedText[end];
                    end += 1;
                }
                if (command.Contains("{CE")) {
                    var information = dialogues[dialogueKey + "_" + numPanel];
                    information["mood"] = command.Split('_')[1];
                    SetProfileInformation(information);
                }
                if (command.Contains("{CS") || command.Contains("{WS") || command.Contains("{CE"))
                    NeedText = NeedText.Remove(position, end - position + 1);
                else
                    position += 1;
            } else {
                position += 1;
            }
        }
    }

    public void LocalizeText()
    {
        int position = 0;
        while (NeedText.Length != position) {
            if (NeedText[position] == '{') {
                int end = position + 1;
                var command = "{";
                while (NeedText[end] != '}') {
                    command += NeedText[end];
                    end += 1;
                }
                if (command.Contains("{CS") || command.Contains("{WS") || command.Contains("{CE"))
                    NeedText = NeedText.Remove(position, end - position + 1);
                else
                    position += 1;
            } else {
                position += 1;
            }
        }
        if (MainText != null)
            MainText.text = NeedText;
    }
}
