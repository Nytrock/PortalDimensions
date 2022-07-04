using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskPortalControl : MonoBehaviour
{
    public Portal portal;

    void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Rigidbody2D _))
        {
            portal.Mask.SetActive(true);
            portal.Teleport = true;
        }
    }
    void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Rigidbody2D _))
        {
            portal.Mask.SetActive(false);
            portal.Teleport = false;
        }
    }
}
