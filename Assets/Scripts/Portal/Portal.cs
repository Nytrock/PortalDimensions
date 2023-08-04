using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Portal : MonoBehaviour
{
    public SpriteRenderer portalSprite;
    private bool right;

    public GameObject masks;
    public ParticleSystem particles;

    public PortalGun gun;
    public string side;

    public BoxCollider2D Collider1;
    public BoxCollider2D Collider2;
    public PortalTrigger trigger;
    public bool Active;
    private Animator animator;

    [Header("Id слоёв сортировки")]
    public int defaultId;
    public int orangeId;
    public int blueId;

    [Header("Звуки")]
    [SerializeField] private AudioSource teleportSound;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Start", true);

        int layer = LayerMask.NameToLayer("PortalWall");
        // Collider.gameObject.layer = layer;

        int newId = orangeId;
        if (right)
            newId = blueId;
        foreach (SpriteMask mask in masks.GetComponentsInChildren<SpriteMask>()) {
            mask.frontSortingLayerID = newId;
            mask.backSortingLayerID = newId;
        }

        Color portalColor = gun.player.leftColor;
        Sprite portalNewSprite = gun.player.leftPortal;
        if (right)  {
            portalNewSprite = gun.player.rightPortal;
            portalColor = gun.player.rightColor;
        }

        ParticleSystem.MainModule main = particles.main;
        portalSprite.sprite = portalNewSprite;
        foreach (Light2D light in portalSprite.GetComponentsInChildren<Light2D>())
            light.color = portalColor;
        main.startColor = portalColor;
    }

    public void SetPortal(bool side, PortalGun newGun)
    {
        gun = newGun;
        right = side;
    }

    public void ActivatePortal(Collider2D itemToTeleport)
    {
        if (right)
            gun.Move_To_Portal(gun.orangePortal, gun.bluePortal, itemToTeleport);
        else
            gun.Move_To_Portal(gun.bluePortal, gun.orangePortal, itemToTeleport);
    }

    public void SetPortalLayer()
    {
        string endLayer = "Blue";
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

    public bool GetRight()
    {
        return right;
    }
}
