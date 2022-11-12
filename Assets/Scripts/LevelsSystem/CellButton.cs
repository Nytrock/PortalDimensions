using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class CellButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button button;
    public Image blur;

    private bool mouseDown;
    private bool pointerDown;

    private void Start()
    {
        blur.color = new Color(1f, 1f, 1f, 0f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && pointerDown) {
            mouseDown = true;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0) && mouseDown) {
            mouseDown = false;
            if (!pointerDown)
                UpdateBlur();
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        pointerDown = true;
        UpdateBlur();
    }
    public void OnPointerExit(PointerEventData eventData) {
        pointerDown = false;
        UpdateBlur();
    }

    IEnumerator BlurVisible()
    {
        for (int i=1; i <= 8; i++) {
            blur.color = new Color(1f, 1f, 1f, 1f / 10f * i);
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
    IEnumerator BlurNonVisible()
    {
        for (int i = 8; i >= 0; i--) {
            blur.color = new Color(1f, 1f, 1f, 1f / 10f * i);
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }

    private void UpdateBlur()
    {
        if (pointerDown)
            StartCoroutine(BlurVisible());
        else if (!mouseDown)
            StartCoroutine(BlurNonVisible());
    }
}
