using System.Collections.Generic;
using UnityEngine;

public class GroundTrigger : MonoBehaviour
{
    private PlayerStateManager player;
    private MapManager mapManager;
    [SerializeField] private List<Collider2D> nowColliders = new();

    private Color nowColor = new();

    private void Awake()
    {
        player = GetComponentInParent<PlayerStateManager>();
    }

    private void Start()
    {
        mapManager = MapManager.mapManager;
        Update_Ground();
    }

    private void Update_Ground()
    {
        player.onGround = nowColliders.Count != 0;
    }

    private void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.gameObject.layer == 3) {
            if (!nowColliders.Contains(obj)) {
                nowColliders.Add(obj);
                Update_Ground();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.gameObject.layer == 3) {
            if (nowColliders.Contains(obj)) {
                nowColliders.Remove(obj);
                Update_Ground();
            }
        }
    }

    private void Update()
    {
        TileData data = mapManager.GetTileData(transform.position);
        if (data == null)
            return;

        if (data.color != nowColor)
            player.animations.Update_Ground(data.color, data.walkAudio, player.GetRigidbody().velocity.y);
    }
}
