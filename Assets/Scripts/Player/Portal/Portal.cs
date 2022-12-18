using UnityEngine;

public class Portal : MonoBehaviour
{
    public GameObject Orange;
    public GameObject Blue;

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

    [Header("Маски")]
    public Transform mask1;
    public Transform mask2;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Start", true);

        int layer = LayerMask.NameToLayer("PortalWall");
        Collider.gameObject.layer = layer;
    }

    public void SetPortal(bool right, PortalGun newGun)
    {
        Orange.SetActive(!right);
        Blue.SetActive(right);
        gun = newGun;
    }

    public void ActivatePortal(Collider2D itemToTeleport)
    {
        if (Blue.activeSelf)
            gun.Move_To_Portal(gun.OrangePortal, gun.BluePortal, itemToTeleport);
        else
            gun.Move_To_Portal(gun.BluePortal, gun.OrangePortal, itemToTeleport);
    }

    public void SetPortalLayer()
    {
        string endLayer = "Orange";
        if (gun.BluePortal.Collider == gun.OrangePortal.Collider && Blue.activeSelf)
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
