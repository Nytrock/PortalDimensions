using UnityEngine;

public class ItemToteleport : MonoBehaviour
{
    private bool teleport;
    private string layerEnd;
    public Portal portal;

    private void Start()
    {
        SetLayer("ItemToTeleport" + layerEnd, true, portal.GetRight());
    }

    private void OnDestroy()
    {
        SetLayer("Default", false, portal.GetRight(), true);
    }

    public void ChangeTeleport(bool newValue)
    {
        teleport = newValue;
        if (teleport)
            SetLayer("TeleportingItem" + layerEnd, true, portal.GetRight());
        else
            SetLayer("ItemToTeleport" + layerEnd, true, portal.GetRight());
    }

    public bool GetTeleport()
    {
        return teleport;
    }

    public void SetLayer(string name, bool visible, bool right, bool destroy=false)
    {
        var list = GetComponentsInChildren<SpriteRenderer>();

        var inter = SpriteMaskInteraction.None;
        if (visible)
            inter = SpriteMaskInteraction.VisibleOutsideMask;

        var newSorting = portal.orangeId;
        if (right)
            newSorting = portal.blueId;
        if (destroy)
            newSorting = portal.defaultId;

        foreach (SpriteRenderer spriteRenderer in list) {
            spriteRenderer.maskInteraction = inter;
            spriteRenderer.sortingLayerID = newSorting;
        }

        int layer = LayerMask.NameToLayer(name);
        gameObject.layer = layer;
        if (TryGetComponent(out PlayerStateManager player)) {
            player.inPortal = !destroy;
            player.animations.powerParticle.GetComponent<ParticleSystemRenderer>().maskInteraction = inter;
            player.animations.powerParticle.GetComponent<ParticleSystemRenderer>().sortingLayerID = newSorting;
            player.animations.teleport.GetComponent<ParticleSystemRenderer>().maskInteraction = inter;
            player.animations.teleport.GetComponent<ParticleSystemRenderer>().sortingLayerID = newSorting;
            player.GetComponentInChildren<GroundTrigger>().gameObject.layer = layer;
            player.GetComponentInChildren<EdgeCollider2D>().gameObject.layer = layer;
        }

    }

    public void SetLayerEnd(string newValue)
    {
        layerEnd = newValue;
    }
}
