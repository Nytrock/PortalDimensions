using UnityEngine;
using UnityEngine.Tilemaps;

public class ShootCollider : MonoBehaviour
{
    private PortalShoot shoot;
    public string side;

    private void Start()
    {
        shoot = GetComponentInParent<PortalShoot>();
    }

    private void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out TilemapCollider2D _))
            shoot.ChangeListColiders(this);
    }

    private void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out TilemapCollider2D _))
            shoot.ChangeListColiders(this, false);
    }
}
