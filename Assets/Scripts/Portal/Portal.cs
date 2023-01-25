using UnityEngine;

public class Portal : MonoBehaviour
{
    public GameObject orange;
    public GameObject blue;

    public PolygonCollider2D Collider;
    public GameObject masks;
    public ParticleSystem Particles;

    public PortalGun gun;
    public string side;

    public BoxCollider2D Collider1;
    public BoxCollider2D Collider2;
    public PortalTrigger trigger;
    public bool Active;
    private Animator animator;

    [Header("Звуки")]
    [SerializeField] private AudioSource teleportSound;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Start", true);

        int layer = LayerMask.NameToLayer("PortalWall");
        Collider.gameObject.layer = layer;

        foreach (SpriteMask mask in masks.GetComponentsInChildren<SpriteMask>()) {
            Debug.Log(mask.isCustomRangeActive);
        }

        orange.GetComponent<SpriteRenderer>().sprite = gun.player.leftPortal;
        blue.GetComponent<SpriteRenderer>().sprite = gun.player.rightPortal;
    }

    public void SetPortal(bool right, PortalGun newGun)
    {
        orange.SetActive(!right);
        blue.SetActive(right);
        gun = newGun;
    }

    public void ActivatePortal(Collider2D itemToTeleport)
    {
        if (blue.activeSelf)
            gun.Move_To_Portal(gun.OrangePortal, gun.BluePortal, itemToTeleport);
        else
            gun.Move_To_Portal(gun.BluePortal, gun.OrangePortal, itemToTeleport);
    }

    public void SetPortalLayer()
    {
        string endLayer = "Orange";
        if (gun.BluePortal.Collider == gun.OrangePortal.Collider && blue.activeSelf)
            endLayer = "Blue";
        int layer = LayerMask.NameToLayer("Portal" + endLayer);
        Collider1.gameObject.layer = layer;
        Collider2.gameObject.layer = layer;
        masks.layer = layer;
        trigger.gameObject.layer = layer;
        trigger.SetLayer(endLayer);
    }

    public void AnimatorPortal()
    {
        animator.SetBool("Active", !animator.GetBool("Active"));
        animator.SetBool("Start", false);
    }

    public void DestroyPortalAnimation()
    {
        animator.SetBool("Death", true);
        if (gun.BluePortal != null && gun.OrangePortal != null) {
            if (gun.BluePortal.Collider != gun.OrangePortal.Collider) {
                int layer = LayerMask.NameToLayer("Ground");
                Collider.gameObject.layer = layer;
            }
        } else {
            int layer = LayerMask.NameToLayer("Ground");
            Collider.gameObject.layer = layer;
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
