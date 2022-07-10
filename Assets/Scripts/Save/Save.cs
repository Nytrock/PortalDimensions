using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;

public class Save : MonoBehaviour
{
    public bool AutoRestart;
    public TextAsset SaveFile;
    public const int NumberLanguages = 2;
    public const string WayToSavefile = "/save/PortalDimensionsSave.txt";
    public LocalizationManager localizationManager;

    [Header("Профили для диалогов")]
    public List<ProfileDialogue> DialogueProfiles;

    [System.Serializable]
    private class SettingsSave
    {
        public int languageId;
    }

    public void Awake()
    {
        Load();
    }

    public void SaveSettings()
    {
        SettingsSave settings = new SettingsSave();
        settings.languageId = LocalizationManager.SelectedLanguage;
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
        } finally {
            stream.Close();
        }
    }
}
