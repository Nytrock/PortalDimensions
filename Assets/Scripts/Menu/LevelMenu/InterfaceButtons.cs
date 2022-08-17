using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class InterfaceButtons : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Animator canvas;
    public void OnPointerEnter(PointerEventData eventData)
    {
        DialogueManager.isButton = true;
        PortalGun.menuActive = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DialogueManager.isButton = false;
        PortalGun.menuActive = false;
    }
}
