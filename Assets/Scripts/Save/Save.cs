using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;

public class Save : MonoBehaviour
{
    public static Save save;

    public const int NumberLanguages = 2;
    public const string WayToSavefile = "/save/PortalDimensionsSave.pd";
    public FpsCounter fpsCounter;
    public LocalizationManager localizationManager;
    public GameSettingsManager gameSettingsManager;
    public ControllSettingsManager controllSettingsManager;
    public AudioSettingsManager audioSettingsManager;
    public VideoSettingsManager videoSettingsManager;
    public DialogueChoiceManager dialogueChoiceManager;

    [Header("Деньги")]
    public MoneyManager moneyManager;

    [Header("Профили для диалогов")]
    public ProfileDialogue[] dialogueProfiles;

    [Header("Списки для вариантов выбора диалогов")]
    [SerializeField] private bool[] ExistingChoiceIdList;
    [SerializeField] private bool[] DoChoiceIdList;

    [Header("Бинды кнопок")]
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode jumpKey;
    public KeyCode portalGunLeftKey;
    public KeyCode portalGunRightKey;
    public KeyCode dialogueStartKey;
    public KeyCode fastRestartKey;

    [Header("Миры")]
    public World[] worlds;

    [Header("Игроки")]
    public Character[] characters;

    [Serializable]
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
    [Serializable]
    private class DialoguesSave
    {
        public bool[] existingChoices;
        public bool[] doingChoices;
    }

    [Serializable]
    private class LevelsSave
    {
        public int numMoney;
    }


    public void Awake()
    {
        Application.targetFrameRate = 60;
        save = this;
        LoadSave();
        LoadLists();
    }

    public void SaveAll()
    {
        SettingsSave settings = new();
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

        DialoguesSave dialogues = new();
        dialogues.existingChoices = ExistingChoiceIdList;
        dialogues.doingChoices = DoChoiceIdList;

        LevelsSave levels = new();
        if (moneyManager)
            levels.numMoney = moneyManager.GetCoins();

        if (!Directory.Exists(Application.dataPath + "/save"))
            Directory.CreateDirectory(Application.dataPath + "/save");

        FileStream stream = new(Application.dataPath + WayToSavefile, FileMode.Create);
        BinaryFormatter form = new();
        form.Serialize(stream, settings);
        form.Serialize(stream, dialogues);
        form.Serialize(stream, levels);
        stream.Close();
    }

    void LoadSave()
    {
        if (File.Exists(Application.dataPath + WayToSavefile)) {
            FileStream stream = new(Application.dataPath + WayToSavefile, FileMode.Open);
            BinaryFormatter form = new();

            try {
                SettingsSave settings = (SettingsSave)form.Deserialize(stream);
                localizationManager.SetLanguage(settings.languageId);
                if (gameSettingsManager) {
                    gameSettingsManager.cursorId = settings.cursorId;
                    if (gameSettingsManager.lightVersion) {
                        gameSettingsManager.SetFps(settings.FpsShowing);
                        gameSettingsManager.SetGlitch(settings.shaderOn);
                    } else {
                        gameSettingsManager.autoManager.isOn = settings.AutoRestart;
                        gameSettingsManager.fpsManager.isOn = settings.FpsShowing;
                        gameSettingsManager.confirmManager.isOn = settings.ConfimToExitActive;
                        gameSettingsManager.glitchManager.isOn = settings.shaderOn;
                    }
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
                    audioSettingsManager.SetValues(settings.volumeMusic, settings.volumeEffects, settings.volumeUI);
                }

                if (videoSettingsManager) {
                    videoSettingsManager.resolutionId = settings.screenResolutionId;
                    videoSettingsManager.modId = settings.screenModId;
                }

                DialoguesSave dialogues = (DialoguesSave)form.Deserialize(stream);
                if (dialogues.existingChoices.Length != 0)
                    ExistingChoiceIdList = dialogues.existingChoices;
                if (dialogues.doingChoices.Length != 0)
                    DoChoiceIdList = dialogues.doingChoices;
                if (dialogueChoiceManager){
                    for (int i = 0; i < DoChoiceIdList.Length; i++) {
                        if (DoChoiceIdList[i])
                            dialogueChoiceManager.DoSomethingFromId(i);
                    }
                }

                LevelsSave levels = (LevelsSave)form.Deserialize(stream);
                if (moneyManager)
                    moneyManager.SetCoins(levels.numMoney);
            } catch {
                stream.Close();
            } finally {
                stream.Close();
            }
        } else {
            if (!Directory.Exists(Application.dataPath + "/save"))
                Directory.CreateDirectory(Application.dataPath + "/save");

            FileStream stream = new(Application.dataPath + WayToSavefile, FileMode.Create);
            stream.Close();
        }
    }

    private void LoadLists()
    {
        var loadedProfiles = Resources.LoadAll<ProfileDialogue>("DialogueProfile");
        dialogueProfiles = new ProfileDialogue[loadedProfiles.Length];
        foreach (ProfileDialogue profile in loadedProfiles)
            dialogueProfiles[profile.id] = profile;

        var loadedWorlds = Resources.LoadAll<World>("Worlds");
        worlds = new World[loadedWorlds.Length];
        foreach (World world in loadedWorlds)
            worlds[world.id] = world;

        var loadedCharacters = Resources.LoadAll<Character>("Characters");
        characters = new Character[loadedCharacters.Length];
        foreach (Character character in loadedCharacters)
            characters[character.id] = character;
    }

    public static bool GetAutoRestart()
    {
        FileStream stream = new(Application.dataPath + WayToSavefile, FileMode.Open);
        BinaryFormatter form = new();
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
        FileStream stream = new(Application.dataPath + WayToSavefile, FileMode.Open);
        BinaryFormatter form = new();
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
        foreach (World world in worlds) {
            world.completedLevels = 1;
            foreach (Level level in world.levels) {
                level.bestScore = 0;
                level.wasCompleted = false;
            }
        }

        foreach (Character character in characters)
            character.available = character.id == 0;

        Array allKeyTypes = Enum.GetValues(typeof(KeyCode));
        SettingsSave settings = new()
        {
            languageId = LocalizationManager.SelectedLanguage,
            cursorId = gameSettingsManager.cursorId,
            AutoRestart = gameSettingsManager.autoManager.isOn,
            FpsShowing = gameSettingsManager.fpsManager.isOn,
            ConfimToExitActive = gameSettingsManager.confirmManager.isOn,
            shaderOn = gameSettingsManager.glitchManager.isOn,
            keyLeft = Array.IndexOf(allKeyTypes, leftKey),
            keyRight = Array.IndexOf(allKeyTypes, rightKey),
            keyJump = Array.IndexOf(allKeyTypes, jumpKey),
            keyLeftPortal = Array.IndexOf(allKeyTypes, portalGunLeftKey),
            keyRightPortal = Array.IndexOf(allKeyTypes, portalGunRightKey),
            keyDialogue = Array.IndexOf(allKeyTypes, dialogueStartKey),
            keyRestart = Array.IndexOf(allKeyTypes, fastRestartKey),
            volumeMusic = audioSettingsManager.musicSlider.value,
            volumeEffects = audioSettingsManager.effectsSlider.value,
            volumeUI = audioSettingsManager.uiSlider.value,
            screenResolutionId = videoSettingsManager.resolutionId,
            screenModId = videoSettingsManager.modId
        };

        if (!Directory.Exists(Application.dataPath + "/save"))
            Directory.CreateDirectory(Application.dataPath + "/save");

        FileStream stream = new(Application.dataPath + WayToSavefile, FileMode.Create);
        BinaryFormatter form = new();
        form.Serialize(stream, settings);
        stream.Close();
    }
}
