using UnityEngine;

public class PortalChecker : MonoBehaviour
{
    public PortalGun gun;

    private void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.gameObject.layer == 3 && obj.TryGetComponent(out GroundGet ground))
        {
            gun.InWall = true;
        }
    }

    private void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.gameObject.layer == 3 && obj.TryGetComponent(out GroundGet ground))
        {
            gun.InWall = false;
        }
    }
}
