using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Save : MonoBehaviour
{
    public TextAsset SaveFile;
    public const int NumberLanguages = 2;
    public const string WayToSavefile = "/save/PortalDimensionsSave.txt";
    public LocalizationManager localizationManager;
    public SettingsManager settingsnManager;

    [Header("Профили для диалогов")]
    public List<ProfileDialogue> DialogueProfiles;

    [System.Serializable]
    private class SettingsSave
    {
        public int languageId;
        public bool AutoRestart;
    }

    public void Awake()
    {
        Load();
    }

    public void SaveSettings()
    {
        SettingsSave settings = new SettingsSave();
        settings.languageId = LocalizationManager.SelectedLanguage;
        settings.AutoRestart = settingsnManager.Autorestart;
        FileStream stream = new FileStream(Application.dataPath + WayToSavefile, FileMode.Create);
        BinaryFormatter form = new BinaryFormatter();
        form.Serialize(stream, settings);
        stream.Close();
    }

    void Load()
    {
        if (File.Exists(Application.dataPath + WayToSavefile)) {
            LoadSettings();

        }
    }

    void LoadSettings()
    {
        FileStream stream = new FileStream(Application.dataPath + WayToSavefile, FileMode.Open);
        BinaryFormatter form = new BinaryFormatter();
        try {
            SettingsSave settings = (SettingsSave)form.Deserialize(stream);
            localizationManager.SetLanguage(settings.languageId);
            if (SceneManager.GetActiveScene().buildIndex == 0)
            settingsnManager.autoManager.isOn = settings.AutoRestart;
        } finally {
            stream.Close();
        }
    }

    public static bool GetAutoRestart()
    {
        FileStream stream = new FileStream(Application.dataPath + WayToSavefile, FileMode.Open);
        BinaryFormatter form = new BinaryFormatter();
        try
        {
            SettingsSave settings = (SettingsSave)form.Deserialize(stream);
            var result = settings.AutoRestart;
            return result;
        }
        finally
        {
            stream.Close();
        }
    }
}
