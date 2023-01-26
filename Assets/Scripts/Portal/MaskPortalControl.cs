using UnityEngine;

public class MaskPortalControl : MonoBehaviour
{
    [SerializeField] private Portal portal;

    private void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out ItemToteleport item))
            item.ChangeTeleport(true);
    }
    private void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out ItemToteleport item)) {
            if (portal == item.portal) {
                item.ChangeTeleport(false);
            }
        }
    }
}
