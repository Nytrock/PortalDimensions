using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class CellButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button button;
    public Image blur;

    public void Start()
    {
        blur.color = new Color(1f, 1f, 1f, 0f);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        StartCoroutine(BlurVisible());
    }
    public void OnPointerExit(PointerEventData eventData) {
        StartCoroutine(BlurNonVisible());
    }

    IEnumerator BlurVisible()
    {
        for (int i=1; i <= 10; i++) {
            blur.color = new Color(1f, 1f, 1f, 1f / 10f * i);
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
    IEnumerator BlurNonVisible()
    {
        for (int i = 10; i >= 0; i--) {
            blur.color = new Color(1f, 1f, 1f, 1f / 10f * i);
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
}
