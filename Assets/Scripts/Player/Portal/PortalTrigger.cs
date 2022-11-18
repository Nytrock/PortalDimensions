using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    public bool inPortal;
    public Portal portal;

    private void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Rigidbody2D _) && !obj.isTrigger) {
            inPortal = true;
            portal.gun.itemToTeleport = obj;
            portal.Update_Portal();
            portal.ChangePregrads(!inPortal);
        }
        if (obj.TryGetComponent(out GroundGet _))
            Destroy(portal.gameObject);
    }

    private void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Rigidbody2D _) && !obj.isTrigger){
            inPortal = false;
            portal.Update_Portal();
            portal.ChangePregrads(!inPortal);
        }
    }
}
