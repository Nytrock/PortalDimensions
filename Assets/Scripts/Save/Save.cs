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
    public GameSettingsManager settingsnManager;
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
        public bool AutoRestart;
        public bool FpsShowing;
        public bool ConfimToExitActive;
    }
    [System.Serializable]
    private class DialoguesSave
    {
        public List<bool> existingChoices;
        public List<bool> doingChoices;
    }


    public void Awake()
    {
        save = this;
        Load();
        dialogueStartKey = KeyCode.E;
    }

    public void SaveAll()
    {
        SettingsSave settings = new SettingsSave();
        settings.languageId = LocalizationManager.SelectedLanguage;
        settings.AutoRestart = settingsnManager.autorestart;
        settings.FpsShowing = settingsnManager.fpsShowing;
        settings.ConfimToExitActive = settingsnManager.confirm;

        DialoguesSave dialogues = new DialoguesSave();
        dialogues.existingChoices = ExistingChoiceIdList;
        dialogues.doingChoices = DoChoiceIdList;

        FileStream stream = new FileStream(Application.dataPath + WayToSavefile, FileMode.Open);
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
                if (settingsnManager) {
                    settingsnManager.autoManager.isOn = settings.AutoRestart;
                    settingsnManager.fpsManager.isOn = settings.FpsShowing;
                    settingsnManager.confirmManager.isOn = settings.ConfimToExitActive;
                }
                fpsCounter.ChangeWorking(settings.FpsShowing);

                DialoguesSave dialogues = (DialoguesSave)form.Deserialize(stream);
                ExistingChoiceIdList = dialogues.existingChoices;
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
}
