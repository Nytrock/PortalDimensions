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
    private Animator animator;
    public List<Collider2D> Pregrads;

    [Header("Звуки")]
    [SerializeField] private AudioSource teleportSound;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Start", true);
    }

    public void SetPortal(bool right, PortalGun newGun)
    {
        Mask.SetActive(false);
        Orange.SetActive(!right);
        Blue.SetActive(right);
        gun = newGun;
    }

    public void Update_Portal()
    {
        if (gun.itemToTeleport) {
            if (gun.itemToTeleport.TryGetComponent(out Player player))
                player.inPortal = trigger.inPortal;
        }

        Mask.SetActive(trigger.inPortal);
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

    public void ChangePregrads(bool Const)
    {
        for (int i = 0; i < Pregrads.Count; i++)
        {
            if (Pregrads[i] != Collider)
                Pregrads[i].enabled = Const;
        }
    }

    public void DestroyPortal()
    {
        Destroy(gameObject);
    }

    public void PlayTeleport()
    {
        teleportSound.Play();
    }
}
