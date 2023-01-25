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
    private CellButton cellButton;
    private Button button;

    public void Awake()
    {
        leftArrowPos = leftArrow.localPosition.x;
        smallBlurPos = smallBlur.transform.localPosition.x;
        cellButton = GetComponent<CellButton>();
        button = GetComponent<Button>();
    }
    public void SetText(string keyText)
    {
        text.text = keyText;
        UpdateLeftArrowAndBlur();    
    }

    public void UpdateLeftArrowAndBlur()
    {

        leftArrow.gameObject.SetActive(false);
        rightArrow.gameObject.SetActive(false);

        StartCoroutine(SetLeftArrow());

        SetBlur(text.text.Length);
    }

    private void SetBlur(int leght) {
        smallBlur.transform.localPosition = new Vector2(smallBlurPos, smallBlur.transform.localPosition.y);
        hugeBlur.color = new Color(hugeBlur.color.r, hugeBlur.color.g, hugeBlur.color.b, 0);
        bigBlur.color = new Color(bigBlur.color.r, bigBlur.color.g, bigBlur.color.b, 0);
        smallBlur.color = new Color(smallBlur.color.r, smallBlur.color.g, smallBlur.color.b, 0);
        mediumBlur.color = new Color(mediumBlur.color.r, mediumBlur.color.g, mediumBlur.color.b, 0);

        if (leght == 2) {
            smallBlur.transform.localPosition = new Vector2(smallBlurPos - 12f, smallBlur.transform.localPosition.y);
        }

        if (leght <= 2) {
            cellButton.blur = smallBlur;
        }  else if (leght <= 4) {
            cellButton.blur = mediumBlur;
        } else if (leght <= 6) {
            cellButton.blur = bigBlur;
        } else {
            cellButton.blur = hugeBlur;
        }
    }

    IEnumerator SetLeftArrow()
    {
        yield return new WaitForSecondsRealtime(0.0001f);
        float percent = text.fontSize / text.fontSizeMax;
        leftArrow.localPosition = new Vector2(leftArrowPos - text.preferredWidth * percent + 22f, leftArrow.localPosition.y);
    }

    public void SetButtonActive(bool value)
    {
        button.interactable = value;
        cellButton.enabled = value;
    }
}
