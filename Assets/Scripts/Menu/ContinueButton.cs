using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public class ContinueButton : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Choice choice;
    void Start()
    {
        if (File.Exists(Application.dataPath + Save.WayToSavefile)) {
            this.GetComponent<Button>().interactable = true;
            text.color = new Color(26f / 256f, 251f / 256f, 255f / 256f);
        } else {
            this.GetComponent<Button>().interactable = false;
            this.GetComponent<ChoiceButton>().enabled = false;
            var index = choice.Buttons.IndexOf(this.GetComponent<Button>());
            for (int i = 1; i < choice.Buttons.Count; i++)
                choice.Buttons[i].GetComponent<ChoiceButton>().id -= 1;
            choice.positions.RemoveAt(index);
            choice.Buttons.RemoveAt(index);
            text.color = new Color(0, 160f / 256f, 163f / 256f);
        }
    }
}
