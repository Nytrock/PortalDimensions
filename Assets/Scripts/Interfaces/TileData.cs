using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(menuName = "Interfaces/TileData")]
public class TileData : ScriptableObject
{

    public TileBase[] tiles;

    public Color color;
    public AudioClip walkAudio;
    public bool forPortal;
}
