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

    private Vector2 VectorForce;
    private float ForceIteration;
    public Collider2D itemToTeleport;

    public void Update_Portal()
    {
        if (itemToTeleport.TryGetComponent(out Player player))
            player.InPortal = trigger.inPortal;
        if (gun.BluePortal.Collider == gun.OrangePortal.Collider)
        {
            if (Blue.activeSelf) {
                gun.OrangePortal.Collider1.enabled = false;
                gun.OrangePortal.Collider2.enabled = false;
            } else
            {
                gun.BluePortal.Collider1.enabled = false;
                gun.BluePortal.Collider2.enabled = false;
            }

        }
        if (Teleport && !trigger.inPortal) {
            if (Blue.activeSelf)
                Move_To_Portal(gun.OrangePortal, gun.BluePortal, itemToTeleport);
            else
                Move_To_Portal(gun.BluePortal, gun.OrangePortal, itemToTeleport);
        } else {
            Collider.enabled = !trigger.inPortal;
            Collider1.enabled = trigger.inPortal;
            Collider2.enabled = trigger.inPortal;
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

    public void Move_To_Portal(Portal Exit, Portal Enter, Collider2D item)
    {
        float Force = 110f;
        ForceIteration = 60f;

        Enter.trigger.inPortal = false;
        Enter.Teleport = false;

        float x = item.bounds.extents.x;
        float y = item.bounds.extents.y;
        switch (Exit.side)
        {
            case "Left": item.transform.position = new Vector2(Exit.transform.position.x + x, Exit.transform.position.y); break;
            case "Down": item.transform.position = new Vector2(Exit.transform.position.x, Exit.transform.position.y + y); break;
            case "Right": item.transform.position = new Vector2(Exit.transform.position.x - x, Exit.transform.position.y); break;
            case "Up": item.transform.position = new Vector2(Exit.transform.position.x, Exit.transform.position.y - y); break;
        }


        Exit.trigger.inPortal = true;
        Exit.Teleport = false;
        Exit.Update_Portal();
        Exit.ChangePregrads(false);
        Exit.Mask.SetActive(true);
        Exit.AnimatorPortal();
        Exit.Collider1.enabled = true;
        Exit.Collider2.enabled = true;
        float velX = (Mathf.Abs(item.GetComponent<Rigidbody2D>().velocity.x) + 1f) / 10f;
        if (velX < 1.2f)
            velX = 1.2f;
        float velY = (Mathf.Abs(item.GetComponent<Rigidbody2D>().velocity.y) + 1f) / 10f;
        if (velY < 1.2f)
            velY = 1.2f;
        item.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        switch (Exit.side)
        {
            case "Left": VectorForce = new Vector2(-Force * velX, 0); break;
            case "Down": VectorForce = new Vector2(0, -Force * velY); break;
            case "Right": VectorForce = new Vector2(Force * velX * 3f, 0); break;
            case "Up": VectorForce = new Vector2(0, Force * velY); break;
        }
        if (item.TryGetComponent(out Player player))
            player.Animations.From_Portal(Exit.Blue.activeSelf);
        else
            item.transform.rotation = Quaternion.Euler(0f, 0f, Exit.transform.rotation.eulerAngles.z);
        StartCoroutine(GiveForce());
    }

    IEnumerator GiveForce()
    {
        while (ForceIteration > 0)
        {
            itemToTeleport.GetComponent<Rigidbody2D>().AddForce(VectorForce / ForceIteration);
            ForceIteration -= 1f;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        itemToTeleport = null;
    }
}
