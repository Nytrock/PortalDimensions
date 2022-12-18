using System.Collections.Generic;
using UnityEngine;

public class JumpTrigger : MonoBehaviour
{
    private Player player;
    private List<Collider2D> nowColliders = new();

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out GroundGet _)) {
            if (!nowColliders.Contains(obj)) {
                nowColliders.Add(obj);
                CanJumpSet();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out GroundGet _))
        {
            if (nowColliders.Contains(obj)) {
                nowColliders.Remove(obj);
                CanJumpSet();
            }
        }
    }

    private void CanJumpSet()
    {
        player.SetJumpingCan(nowColliders.Count != 0);   
    }
}
