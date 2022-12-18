using System.Collections.Generic;
using UnityEngine;

public class GroundTrigger : MonoBehaviour
{
    private Player player;
    public List<Collider2D> nowColliders;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }
    private void LateUpdate()
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
        if (obj.TryGetComponent(out GroundGet ground)) {
            if (!nowColliders.Contains(obj)) {
                nowColliders.Add(obj);
                player.Update_Ground(ground);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out GroundGet _)) {
            if (nowColliders.Contains(obj))
                nowColliders.Remove(obj);
        }
    }
}
