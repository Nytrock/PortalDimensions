using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunGet : MonoBehaviour
{
    public GameObject portalGun;
    private bool canPickUp;

    public void Update()
    {
        if (canPickUp && Input.GetKeyDown(KeyCode.E)) {
            portalGun.SetActive(true);
            gameObject.SetActive(false);
        }
        
    }

    public void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Player player)) {
            canPickUp = true;
        }
    }

    public void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Player player)) {
            canPickUp = false;
        }
    }
}
