using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    public bool inPortal;
    public Portal portal;
    void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Player _))
        {
            inPortal = true;
            portal.Update_Portal();
            portal.ChangePregrads(!inPortal);
        }
        if (obj.TryGetComponent(out GroundGet _))
            Destroy(portal.gameObject);
    }
    void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Player _))
        {
            inPortal = false;
            portal.Update_Portal();
            portal.ChangePregrads(!inPortal);
        }
    }
}
