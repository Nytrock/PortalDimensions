using UnityEngine;

public class PregradsControl : MonoBehaviour
{
    public Portal portal;

    private void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.gameObject.layer == 3 && !obj.CompareTag("Shoot") && !obj.CompareTag("Player") && !portal.Pregrads.Contains(obj)) {
            if (obj.TryGetComponent(out PolygonCollider2D polygon)) {
                if (polygon == portal.Collider)
                    return;
            }
            if (obj.TryGetComponent(out BoxCollider2D box)) {
                if (portal.gun.BluePortal != null) {
                    if (box == portal.gun.BluePortal.Collider1 || box == portal.gun.BluePortal.Collider2)
                        return;
                }
                if (portal.gun.OrangePortal != null) {
                    if (box == portal.gun.OrangePortal.Collider1 || box == portal.gun.OrangePortal.Collider2)
                        return;
                }
            }
            portal.Pregrads.Add(obj);
        }
    }

    private void OnTriggerExit2D(Collider2D obj)
    {
        if (portal.Pregrads.Contains(obj))
            portal.Pregrads.Remove(obj);
    }
}
