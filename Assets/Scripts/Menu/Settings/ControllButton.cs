using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

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
    void Start()
    {
        leftArrowPos = leftArrow.localPosition.x;
        smallBlurPos = smallBlur.transform.localPosition.x;
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

        int leght = text.text.Length;

        if (text.text == "RightAlt" || text.text == "LeftAlt")
            leftArrow.localPosition = new Vector2(leftArrowPos - 16f * (leght - 1), leftArrow.localPosition.y);
        else if (text.text == "KeypadMultiply")
            leftArrow.localPosition = new Vector2(leftArrowPos - 10f * (leght - 1), leftArrow.localPosition.y);
        else if (text.text == "KeypadPlus")
            leftArrow.localPosition = new Vector2(leftArrowPos - 14.5f * (leght - 1), leftArrow.localPosition.y);
        else if (leght < 5 || text.text == "PageUp" || text.text == "SysReq")
            leftArrow.localPosition = new Vector2(leftArrowPos - 20f * (leght - 1), leftArrow.localPosition.y);
        else if (leght == 6 || text.text == "Minus" || text.text == "LeftShift" || text.text == "Backslash" || text.text == "Semicolon")
            leftArrow.localPosition = new Vector2(leftArrowPos - 17f * (leght - 1), leftArrow.localPosition.y);
        else if (leght == 7)
            leftArrow.localPosition = new Vector2(leftArrowPos - 18f * (leght - 1), leftArrow.localPosition.y);
        else if (leght < 10)
            leftArrow.localPosition = new Vector2(leftArrowPos - 19f * (leght - 1), leftArrow.localPosition.y);
        else if (leght == 11)
            leftArrow.localPosition = new Vector2(leftArrowPos - 13f * (leght - 1), leftArrow.localPosition.y);
        else
            leftArrow.localPosition = new Vector2(leftArrowPos - 12f * (leght - 1), leftArrow.localPosition.y);

        smallBlur.transform.localPosition = new Vector2(smallBlurPos, smallBlur.transform.localPosition.y);
        switch (leght) {
            case 2:
                smallBlur.transform.localPosition = new Vector2(smallBlurPos - 12f, smallBlur.transform.localPosition.y);
                break;
        }

        hugeBlur.color = new Color(1f, 1f, 1f, 0);
        bigBlur.color = new Color(1f, 1f, 1f, 0);
        smallBlur.color = new Color(1f, 1f, 1f, 0);
        mediumBlur.color = new Color(1f, 1f, 1f, 0);

        if (leght <= 2) {
            smallBlur.color = new Color(1f, 1f, 1f, 1);
            GetComponent<Button>().targetGraphic = smallBlur;
        } else if (leght <= 4) {
            mediumBlur.color = new Color(1f, 1f, 1f, 1);
            GetComponent<Button>().targetGraphic = mediumBlur;
        } else if (leght >= 7){
            hugeBlur.color = new Color(1f, 1f, 1f, 1);
            GetComponent<Button>().targetGraphic = hugeBlur;
        } else {
            bigBlur.color = new Color(1f, 1f, 1f, 1);
            GetComponent<Button>().targetGraphic = bigBlur;
        }
    }
}
