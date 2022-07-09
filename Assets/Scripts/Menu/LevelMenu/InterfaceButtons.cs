using UnityEngine;
using UnityEngine.EventSystems;

public class InterfaceButtons : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Animator canvas;
    public void OnPointerEnter(PointerEventData eventData)
    {
        PortalGun.menuActive = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PortalGun.menuActive = false;
    }

    public void PauseButton()
    {
        canvas.SetBool("isPause", !canvas.GetBool("isPause"));
    }

    public void RestartButton()
    {
        // Вообще не готово
        canvas.SetBool("isDeath", !canvas.GetBool("isDeath"));
    }
}
