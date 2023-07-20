using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunGet : MonoBehaviour
{

    public void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out PlayerStateManager player))
        {
            player.portalGun.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
