using UnityEngine;

public class MaskPortalControl : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out ItemToteleport item))
            item.ChangeTeleport(true);
    }
    private void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out ItemToteleport item))
            item.ChangeTeleport(false);
    }
}
