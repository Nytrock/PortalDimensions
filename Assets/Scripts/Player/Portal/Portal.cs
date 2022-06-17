using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public GameObject Orange;
    public GameObject Blue;

    public PolygonCollider2D Collider;
    public GameObject Mask;
    public ParticleSystem Particles;

    public PortalGun gun;
    public int side;

    public BoxCollider2D Collider1;
    public BoxCollider2D Collider2;
    public PortalTrigger trigger;
    public bool Teleport;
    public bool Active;
    public Animator animator;
    public List<Collider2D> Pregrads;

    public void Update_Portal()
    {
        gun.player.InPortal = trigger.inPortal;
        if (Teleport && !trigger.inPortal)
        {
            if (Blue.activeSelf)
                gun.Move_To_Portal(gun.Orange, gun.Blue);
            else
                gun.Move_To_Portal(gun.Blue, gun.Orange);
        }
        else
        {
            Collider.enabled = !trigger.inPortal;
        }
    }

    public void AnimatorPortal()
    {
        animator.SetBool("Active", !animator.GetBool("Active"));
    }

    public void ChangePregrads(bool Const)
    {
        for (int i = 0; i < Pregrads.Count; i++)
        {
            if (Pregrads[i] != Collider)
                Pregrads[i].enabled = Const;
        }
    }
}
