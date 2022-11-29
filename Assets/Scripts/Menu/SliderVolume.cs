using UnityEngine;
using UnityEngine.EventSystems;

public class SliderVolume : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool pointerDown;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && pointerDown)
            ButtonFunctional.buttonFunctional.HoverPlay();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerDown = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        pointerDown = false;
    }
}
