using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    public Portal portal;
    private string layer = "Orange";

    private void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Rigidbody2D _) && !obj.isTrigger) {
            if (!obj.TryGetComponent(out ItemToteleport _)) {
                obj.gameObject.AddComponent<ItemToteleport>().SetLayerEnd(layer);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Rigidbody2D _) && !obj.isTrigger){
            if (!obj.GetComponent<ItemToteleport>().GetTeleport())
                Destroy(obj.GetComponent<ItemToteleport>());
            else
                portal.ActivatePortal(obj);
        }
    }

    public void SetLayer(string newValue)
    {
        layer = newValue;
    }
}
