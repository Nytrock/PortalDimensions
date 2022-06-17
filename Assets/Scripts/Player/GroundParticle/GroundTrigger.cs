using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTrigger : MonoBehaviour
{
    public Player player;
    public List<Collider2D> NowColliders;

    public void Update()
    {
        if (NowColliders.Count == 0)
            player.onGround = false;
        else
            player.onGround = true;
    }
    void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.gameObject.layer == 3 && obj.TryGetComponent(out GroundGet ground))
        {
            if (!NowColliders.Contains(obj))
            {
                NowColliders.Add(obj);
                player.Update_Ground(ground);
            }
        }
    }

    void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.gameObject.layer == 3 && obj.TryGetComponent(out GroundGet _))
        {
            if (NowColliders.Contains(obj))
                NowColliders.Remove(obj);
        }
    }
}
