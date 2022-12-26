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
                player.Update_Ground(ground);
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
