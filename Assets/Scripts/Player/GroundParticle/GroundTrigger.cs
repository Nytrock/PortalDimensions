using System.Collections.Generic;
using UnityEngine;

public class GroundTrigger : MonoBehaviour
{
    private Player player;
    public List<Collider2D> nowColliders;

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
        if (nowColliders.Count == 0)
            player.onGround = false;
        else
            player.onGround = true;
    }

    private void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.gameObject.layer == 3 && obj.TryGetComponent(out GroundGet ground)) {
            if (!nowColliders.Contains(obj)) {
                nowColliders.Add(obj);
                Update_Ground();
                player.Update_Ground(ground);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.gameObject.layer == 3 && obj.TryGetComponent(out GroundGet _)) {
            if (nowColliders.Contains(obj))
                nowColliders.Remove(obj);
        }
    }
}
