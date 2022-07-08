using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
public class ContinueButton : MonoBehaviour
{
    public TextMeshProUGUI text;
    void Start()
    {
        if (File.Exists(Application.dataPath + "/save/PortalDimensionsSave.txt")) {
            this.GetComponent<Button>().interactable = true;
            text.color = new Color(26f / 256f, 251f / 256f, 255f / 256f);
        } else {
            this.GetComponent<Button>().interactable = false;
            text.color = new Color(0, 160f / 256f, 163f / 256f);
        }
    }
}
