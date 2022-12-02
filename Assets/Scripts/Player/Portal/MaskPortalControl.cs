using UnityEngine;

public class MaskPortalControl : MonoBehaviour
{
    public Portal portal;

    private void Start()
    {
        if (portal.side == "Down")
            portal.Mask.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Rigidbody2D _))
        {
            portal.Teleport = true;
        }
    }
    private void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Rigidbody2D _)) {
            portal.Teleport = false;
        }
    }
}
