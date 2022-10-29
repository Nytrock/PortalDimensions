using System.Text;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class LocalizationManager : MonoBehaviour
{
    public static int SelectedLanguage { get; private set; }
    public static LocalizationManager localizationManager;

    public static event LanguageChangeHandler OnLanguageChange;
    public delegate void LanguageChangeHandler();

    public static int numLanguages;

    private static Dictionary<string, List<string>> localization;

    [SerializeField]
    private TextAsset textFile;

    private void Awake()
    {
        localizationManager = this;
        if (localization == null)
            LoadLocatization();
    }

    public void SetLanguage(int id)
    {
        SelectedLanguage = id;
        OnLanguageChange?.Invoke();
    }

    private void LoadLocatization()
    {
        localization = new Dictionary<string, List<string>>();

        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(textFile.text);

        foreach (XmlNode key in xmlDocument["keys"].ChildNodes)
        {
            string keyStr = key.Attributes["name"].Value;

            var values = new List<string>();
            foreach (XmlNode translate in key["translates"].ChildNodes)
                values.Add(translate.InnerText);
            numLanguages = values.Count;
            localization[keyStr] = values;
        }
    }

    public static string GetTranslate(string key, int languageId = -1)
    {
        if (languageId == -1)
            languageId = SelectedLanguage;

        if (localization.ContainsKey(key))
            return localization[key][languageId];

        return key;
    }
}
