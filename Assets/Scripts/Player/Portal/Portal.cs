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
    public string side;

    public BoxCollider2D Collider1;
    public BoxCollider2D Collider2;
    public PortalTrigger trigger;
    public bool Teleport;
    public bool Active;
    public Animator animator;
    public List<Collider2D> Pregrads;

    private float TargetScalePortal;
    private float StepScalePortal;
    private float TargetScaleParticles;
    private float StepScaleParticles;

    public void Update_Portal()
    {
        if (gun.itemToTeleport) {
            if (gun.itemToTeleport.TryGetComponent(out Player player))
                player.InPortal = trigger.inPortal;
        }

        if (Teleport && !trigger.inPortal) {
            if (Blue.activeSelf)
                gun.Move_To_Portal(gun.OrangePortal, gun.BluePortal, gun.itemToTeleport);
            else
                gun.Move_To_Portal(gun.BluePortal, gun.OrangePortal, gun.itemToTeleport);
        } else {
            Collider.enabled = !trigger.inPortal;
        }
    }

    public void AnimatorPortal()
    {
        animator.SetBool("Active", !animator.GetBool("Active"));
        animator.SetBool("Start", false);
    }

    public void DestroyPortalAnimation()
    {
        animator.SetBool("Death", true);
    }

    void DestroyPortal()
    {
        Destroy(gameObject);
    }

    public void ChangePregrads(bool Const)
    {
        for (int i = 0; i < Pregrads.Count; i++)
        {
            if (Pregrads[i] != Collider)
                Pregrads[i].enabled = Const;
        }
    }

    public void StartChangingScale()
    {
        TargetScalePortal = Blue.transform.localScale.y;
        Blue.transform.localScale = new Vector2(Blue.transform.localScale.x, 0);
        Orange.transform.localScale = new Vector2(Orange.transform.localScale.x, 0);
        StepScalePortal = TargetScalePortal / 100f;

        TargetScaleParticles = Particles.transform.localScale.x;
        Particles.transform.localScale = new Vector2(0, Particles.transform.localScale.y);
        StepScaleParticles = TargetScaleParticles / 100f;

        StartCoroutine(IncreaseScale());
    }

    IEnumerator IncreaseScale()
    {
        while (TargetScalePortal > Blue.transform.localScale.y)
        {
            Blue.transform.localScale = new Vector2(Blue.transform.localScale.x, Blue.transform.localScale.y + StepScalePortal);
            Orange.transform.localScale = new Vector2(Orange.transform.localScale.x, Orange.transform.localScale.y + StepScalePortal);

            Particles.transform.localScale = new Vector2(Particles.transform.localScale.x + StepScaleParticles, Particles.transform.localScale.y);

            yield return new WaitForSeconds(0.0025f);
        }
    }
}
