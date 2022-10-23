using System.Collections.Generic;
using UnityEngine;

public class GroundTrigger : MonoBehaviour
{
    private Player player;
    public List<Collider2D> NowColliders;

    private void Start()
    {
        player = GetComponentInParent<Player>();
    }
    private void Update()
    {
        Update_Ground();
    }

    private void Update_Ground()
    {
        if (NowColliders.Count == 0)
            player.onGround = false;
        else
            player.onGround = true;
    }

    void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.gameObject.layer == 3 && obj.TryGetComponent(out GroundGet ground)) {
            if (!NowColliders.Contains(obj)) {
                NowColliders.Add(obj);
                Update_Ground();
                player.Update_Ground(ground);
            }
        }
    }

    void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.gameObject.layer == 3 && obj.TryGetComponent(out GroundGet _)) {
            if (NowColliders.Contains(obj))
                NowColliders.Remove(obj);
        }
    }
}
