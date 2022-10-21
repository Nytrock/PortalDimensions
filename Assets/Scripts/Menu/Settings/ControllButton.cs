using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ControllButton : MonoBehaviour
{
    public TextMeshProUGUI text;

    public Image hugeBlur;
    public Image bigBlur;
    public Image mediumBlur;
    public Image smallBlur;

    public Transform leftArrow;
    public Transform rightArrow;

    private float leftArrowPos;
    private float smallBlurPos;

    public void Awake()
    {
        leftArrowPos = leftArrow.localPosition.x;
        smallBlurPos = smallBlur.transform.localPosition.x;
    }
    public void SetText(string keyText)
    {
        text.text = keyText;
        UpdateLeftArrowAndBlur();    
    }

    public void UpdateLeftArrowAndBlur()
    {
        var button = GetComponent<Button>();
        ColorBlock cb = button.colors;
        cb.normalColor = new Color(1f, 1f, 1f, 0f);
        button.colors = cb;

        leftArrow.gameObject.SetActive(false);
        rightArrow.gameObject.SetActive(false);

        StartCoroutine(SetLeftArrow());

        SetBlur(text.text.Length);
    }

    private void SetBlur(int leght) {
        smallBlur.transform.localPosition = new Vector2(smallBlurPos, smallBlur.transform.localPosition.y);
        hugeBlur.color = new Color(1f, 1f, 1f, 0);
        bigBlur.color = new Color(1f, 1f, 1f, 0);
        smallBlur.color = new Color(1f, 1f, 1f, 0);
        mediumBlur.color = new Color(1f, 1f, 1f, 0);

        if (leght == 2) {
            smallBlur.transform.localPosition = new Vector2(smallBlurPos - 12f, smallBlur.transform.localPosition.y);
        }

        if (leght <= 2) {
            smallBlur.color = new Color(1f, 1f, 1f, 1);
            GetComponent<Button>().targetGraphic = smallBlur;
        }  else if (leght <= 4) {
            mediumBlur.color = new Color(1f, 1f, 1f, 1);
            GetComponent<Button>().targetGraphic = mediumBlur;
        } else if (leght <= 6) {
            bigBlur.color = new Color(1f, 1f, 1f, 1);
            GetComponent<Button>().targetGraphic = bigBlur;
        } else {
            hugeBlur.color = new Color(1f, 1f, 1f, 1);
            GetComponent<Button>().targetGraphic = hugeBlur;
        }
    }

    IEnumerator SetLeftArrow()
    {
        yield return new WaitForSecondsRealtime(0.0001f);
        float percent = text.fontSize / text.fontSizeMax;
        leftArrow.localPosition = new Vector2(leftArrowPos - text.preferredWidth * percent + 22f, leftArrow.localPosition.y);
    }
}
