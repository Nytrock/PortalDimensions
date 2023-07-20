using System.Collections.Generic;
using UnityEngine;

public class GroundTrigger : MonoBehaviour
{
    private PlayerStateManager player;
    public List<Collider2D> nowColliders;

    private void Awake()
    {
        player = GetComponentInParent<PlayerStateManager>();
    }

    private void Start()
    {
        Update_Ground();
    }

    private void Update_Ground()
    {
        player.onGround = nowColliders.Count != 0;
    }

    private void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out GroundGet ground)) {
            if (!nowColliders.Contains(obj)) {
                nowColliders.Add(obj);
                Update_Ground();
                player.animations.Update_Ground(ground, player.GetRigidbody().velocity.y);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out GroundGet _)) {
            if (nowColliders.Contains(obj))
            {
                nowColliders.Remove(obj);
                Update_Ground();
            }
        }
    }
}
