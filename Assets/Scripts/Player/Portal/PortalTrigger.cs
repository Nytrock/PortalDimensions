using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    public bool inPortal;
    public Portal portal;
    void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Rigidbody2D _))
        {
            inPortal = true;
            portal.gun.itemToTeleport = obj;
            portal.Update_Portal();
            portal.ChangePregrads(!inPortal);
        }
        if (obj.TryGetComponent(out GroundGet _))
            Destroy(portal.gameObject);
    }
    void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Rigidbody2D _))
        {
            inPortal = false;
            portal.Update_Portal();
            portal.ChangePregrads(!inPortal);
        }
    }
}
