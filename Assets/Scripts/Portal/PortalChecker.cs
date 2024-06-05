using UnityEngine;

public class PortalChecker : MonoBehaviour
{
    public PortalGun gun;

    private void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.gameObject.layer == 3)
            gun.inWall = true;
    }

    private void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.gameObject.layer == 3)
            gun.inWall = false;
    }
}