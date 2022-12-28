using UnityEngine;

public class ItemToteleport : MonoBehaviour
{
    private bool teleport;
    private string layerEnd;
    public Portal portal;

    private void Start()
    {
        var list = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer spriteRenderer in list)
            spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        SetLayer("ItemToTeleport" + layerEnd);
        if (TryGetComponent(out Player player)) {
            player.inPortal = true;
            player.animations.powerParticle.GetComponent<ParticleSystemRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        }
    }

    private void OnDestroy()
    {
        var list = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer spriteRenderer in list)
            spriteRenderer.maskInteraction = SpriteMaskInteraction.None;
        SetLayer("Default");
        if (TryGetComponent(out Player player)) {
            player.inPortal = false;
            player.animations.powerParticle.GetComponent<ParticleSystemRenderer>().maskInteraction = SpriteMaskInteraction.None;
        }
    }

    public void ChangeTeleport(bool newValue)
    {
        teleport = newValue;
        if (teleport)
            SetLayer("TeleportingItem" + layerEnd);
        else
            SetLayer("ItemToTeleport" + layerEnd);
    }

    public bool GetTeleport()
    {
        return teleport;
    }

    public void SetLayer(string name)
    {
        int layer = LayerMask.NameToLayer(name);
        gameObject.layer = layer;
        if (TryGetComponent(out Player player)) {
            player.GetComponentInChildren<GroundTrigger>().gameObject.layer = layer;
            player.GetComponentInChildren<EdgeCollider2D>().gameObject.layer = layer;
        }
    }

    public void SetLayerEnd(string newValue)
    {
        layerEnd = newValue;
    }
}
