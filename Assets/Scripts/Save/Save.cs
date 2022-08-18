using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Save : MonoBehaviour
{
    public static Save save;

    public const int NumberLanguages = 2;
    public const string WayToSavefile = "/save/PortalDimensionsSave.pd";
    public LocalizationManager localizationManager;
    public GameSettingsManager gameSettingsManager;
    public ControllSettingsManager controllSettingsManager;
    public AudioSettingsManager audioSettingsManager;
    public VideoSettingsManager videoSettingsManager;
    public FpsCounter fpsCounter;
    public DialogueChoiceManager dialogueChoiceManager;

    [Header("Профили для диалогов")]
    public List<ProfileDialogue> DialogueProfiles;

    [Header("Списки для вариантов выбора диалогов")]
    [SerializeField] private List<bool> ExistingChoiceIdList;
    [SerializeField] private List<bool> DoChoiceIdList;

    [Header("Бинды кнопок")]
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode jumpKey;
    public KeyCode portalGunLeftKey;
    public KeyCode portalGunRightKey;
    public KeyCode dialogueStartKey;
    public KeyCode fastRestartKey;

    [System.Serializable]
    private class SettingsSave
    {
        public int languageId;
        public int cursorId;
        public bool AutoRestart;
        public bool FpsShowing;
        public bool ConfimToExitActive;
        public bool shaderOn;
        public int keyLeft;
        public int keyRight;
        public int keyJump;
        public int keyLeftPortal;
        public int keyRightPortal;
        public int keyDialogue;
        public int keyRestart;
        public float volumeMusic;
        public float volumeEffects;
        public float volumeUI;
        public int screenResolutionId;
        public int screenModId;
    }
    [System.Serializable]
    private class DialoguesSave
    {
        public List<bool> existingChoices;
        public List<bool> doingChoices;
    }


    public void Awake()
    {
        Application.targetFrameRate = 60;
        save = this;
        Load();
    }

    public void SaveAll()
    {
        SettingsSave settings = new SettingsSave();
        settings.languageId = LocalizationManager.SelectedLanguage;
        settings.cursorId = gameSettingsManager.cursorId;
        settings.AutoRestart = gameSettingsManager.autoManager.isOn;
        settings.FpsShowing = gameSettingsManager.fpsManager.isOn;
        settings.ConfimToExitActive = gameSettingsManager.confirmManager.isOn;
        settings.shaderOn = gameSettingsManager.glitchManager.isOn;

        Array allKeyTypes = Enum.GetValues(typeof(KeyCode));
        settings.keyLeft = Array.IndexOf(allKeyTypes, leftKey);
        settings.keyRight = Array.IndexOf(allKeyTypes, rightKey);
        settings.keyJump = Array.IndexOf(allKeyTypes, jumpKey);
        settings.keyLeftPortal = Array.IndexOf(allKeyTypes, portalGunLeftKey);
        settings.keyRightPortal = Array.IndexOf(allKeyTypes, portalGunRightKey);
        settings.keyDialogue = Array.IndexOf(allKeyTypes, dialogueStartKey);
        settings.keyRestart = Array.IndexOf(allKeyTypes, fastRestartKey);
        settings.volumeMusic = audioSettingsManager.musicSlider.value;
        settings.volumeEffects = audioSettingsManager.effectsSlider.value;
        settings.volumeUI = audioSettingsManager.uiSlider.value;
        settings.screenResolutionId = videoSettingsManager.resolutionId;
        settings.screenModId = videoSettingsManager.modId;

        DialoguesSave dialogues = new DialoguesSave();
        dialogues.existingChoices = ExistingChoiceIdList;
        dialogues.doingChoices = DoChoiceIdList;

        if (!Directory.Exists(Application.dataPath + "/save"))
            Directory.CreateDirectory(Application.dataPath + "/save");

        FileStream stream = new FileStream(Application.dataPath + WayToSavefile, FileMode.Create);
        BinaryFormatter form = new BinaryFormatter();
        form.Serialize(stream, settings);
        form.Serialize(stream, dialogues);
        stream.Close();
    }

    void Load()
    {
        if (File.Exists(Application.dataPath + WayToSavefile)) {
            FileStream stream = new FileStream(Application.dataPath + WayToSavefile, FileMode.Open);
            BinaryFormatter form = new BinaryFormatter();

            try {
                SettingsSave settings = (SettingsSave)form.Deserialize(stream);
                localizationManager.SetLanguage(settings.languageId);
                if (gameSettingsManager) {
                    gameSettingsManager.autoManager.isOn = settings.AutoRestart;
                    gameSettingsManager.cursorId = settings.cursorId;
                    gameSettingsManager.fpsManager.isOn = settings.FpsShowing;
                    gameSettingsManager.confirmManager.isOn = settings.ConfimToExitActive;
                    gameSettingsManager.glitchManager.isOn = settings.shaderOn;
                }

                Array allKeyTypes = Enum.GetValues(typeof(KeyCode));
                if ((KeyCode)allKeyTypes.GetValue(settings.keyLeft) != KeyCode.None)
                    leftKey = (KeyCode)allKeyTypes.GetValue(settings.keyLeft);
                if ((KeyCode)allKeyTypes.GetValue(settings.keyRight) != KeyCode.None)
                    rightKey = (KeyCode)allKeyTypes.GetValue(settings.keyRight);
                if ((KeyCode)allKeyTypes.GetValue(settings.keyJump) != KeyCode.None)
                    jumpKey = (KeyCode)allKeyTypes.GetValue(settings.keyJump);
                if ((KeyCode)allKeyTypes.GetValue(settings.keyLeftPortal) != KeyCode.None)
                    portalGunLeftKey = (KeyCode)allKeyTypes.GetValue(settings.keyLeftPortal);
                if ((KeyCode)allKeyTypes.GetValue(settings.keyRightPortal) != KeyCode.None)
                    portalGunRightKey = (KeyCode)allKeyTypes.GetValue(settings.keyRightPortal);
                if ((KeyCode)allKeyTypes.GetValue(settings.keyDialogue) != KeyCode.None)
                    dialogueStartKey = (KeyCode)allKeyTypes.GetValue(settings.keyDialogue);
                if ((KeyCode)allKeyTypes.GetValue(settings.keyRestart) != KeyCode.None)
                    fastRestartKey = (KeyCode)allKeyTypes.GetValue(settings.keyRestart);
                
                if (audioSettingsManager) {
                    audioSettingsManager.musicSlider.value = settings.volumeMusic;
                    audioSettingsManager.effectsSlider.value = settings.volumeEffects;
                    audioSettingsManager.uiSlider.value = settings.volumeUI;
                }

                if (videoSettingsManager) {
                    videoSettingsManager.resolutionId = settings.screenResolutionId;
                    videoSettingsManager.modId = settings.screenModId;
                }

                DialoguesSave dialogues = (DialoguesSave)form.Deserialize(stream);
                if (dialogues.existingChoices.Count != 0)
                    ExistingChoiceIdList = dialogues.existingChoices;
                if (dialogues.doingChoices.Count != 0)
                    DoChoiceIdList = dialogues.doingChoices;
                if (dialogueChoiceManager){
                    for (int i = 0; i < DoChoiceIdList.Count; i++) {
                        if (DoChoiceIdList[i])
                            dialogueChoiceManager.DoSomethingFromId(i);
                    }
                }
            } catch {
                stream.Close();
            } finally {
                stream.Close();
            }
        } else {
            if (!Directory.Exists(Application.dataPath + "/save"))
                Directory.CreateDirectory(Application.dataPath + "/save");

            FileStream stream = new FileStream(Application.dataPath + WayToSavefile, FileMode.Create);
            stream.Close();
        }
    }

    public static bool GetAutoRestart()
    {
        FileStream stream = new FileStream(Application.dataPath + WayToSavefile, FileMode.Open);
        BinaryFormatter form = new BinaryFormatter();
        try {
            SettingsSave settings = (SettingsSave)form.Deserialize(stream);
            var result = settings.AutoRestart;
            return result;
        } finally {
            stream.Close();
        }
    }

    public static bool GetConfirmNeed()
    {
        FileStream stream = new FileStream(Application.dataPath + WayToSavefile, FileMode.Open);
        BinaryFormatter form = new BinaryFormatter();
        try {
            SettingsSave settings = (SettingsSave)form.Deserialize(stream);
            var result = settings.ConfimToExitActive;
            return result;
        } finally {
            stream.Close();
        }
    }

    public bool GetChoiceExisting(int id)
    {
        return ExistingChoiceIdList[id];
    }

    public void SetChoiceExisting(int id, bool newValue)
    {
        ExistingChoiceIdList[id] = newValue;
    }

    public void SetChoiceDoing(int id, bool newValue)
    {
        DoChoiceIdList[id] = newValue;
    }

    public void ResetGame()
    {
        SettingsSave settings = new SettingsSave();
        settings.languageId = LocalizationManager.SelectedLanguage;
        settings.cursorId = gameSettingsManager.cursorId;
        settings.AutoRestart = gameSettingsManager.autoManager.isOn;
        settings.FpsShowing = gameSettingsManager.fpsManager.isOn;
        settings.ConfimToExitActive = gameSettingsManager.confirmManager.isOn;
        settings.shaderOn = gameSettingsManager.glitchManager.isOn;

        Array allKeyTypes = Enum.GetValues(typeof(KeyCode));
        settings.keyLeft = Array.IndexOf(allKeyTypes, leftKey);
        settings.keyRight = Array.IndexOf(allKeyTypes, rightKey);
        settings.keyJump = Array.IndexOf(allKeyTypes, jumpKey);
        settings.keyLeftPortal = Array.IndexOf(allKeyTypes, portalGunLeftKey);
        settings.keyRightPortal = Array.IndexOf(allKeyTypes, portalGunRightKey);
        settings.keyDialogue = Array.IndexOf(allKeyTypes, dialogueStartKey);
        settings.keyRestart = Array.IndexOf(allKeyTypes, fastRestartKey);
        settings.volumeMusic = audioSettingsManager.musicSlider.value;
        settings.volumeEffects = audioSettingsManager.effectsSlider.value;
        settings.volumeUI = audioSettingsManager.uiSlider.value;
        settings.screenResolutionId = videoSettingsManager.resolutionId;
        settings.screenModId = videoSettingsManager.modId;

        if (!Directory.Exists(Application.dataPath + "/save"))
            Directory.CreateDirectory(Application.dataPath + "/save");

        FileStream stream = new FileStream(Application.dataPath + WayToSavefile, FileMode.Create);
        BinaryFormatter form = new BinaryFormatter();
        form.Serialize(stream, settings);
        stream.Close();
    }
}
