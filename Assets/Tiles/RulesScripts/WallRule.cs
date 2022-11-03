using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/WallRule")]
public class WallRule : RuleTile<WallRule.Neighbor> {
    public bool customField;
    public TileBase portalTile;
    public TileBase protectTile;

    public class Neighbor : RuleTile.TilingRule.Neighbor {
        public const int Portal = 3;
        public const int Protect = 4;
        public const int Nothing = 5;
        public const int Any = 6;
    }

    public override bool RuleMatch(int neighbor, TileBase tile) {
        switch (neighbor) {
            case Neighbor.This: return tile == this; ;
            case Neighbor.NotThis: return tile != this;
            case Neighbor.Portal: return tile == portalTile;
            case Neighbor.Protect: return tile == protectTile;
            case Neighbor.Nothing: return tile == null;
            case Neighbor.Any: return tile != null;
        }
        return base.RuleMatch(neighbor, tile);
    }
}