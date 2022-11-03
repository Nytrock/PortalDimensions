using UnityEngine;

namespace UnityEditor
{
    [CustomEditor(typeof(WallRule))]
    [CanEditMultipleObjects]
    public class NotPortalRuleEditor : RuleTileEditor
    {
        public Texture2D PortalIcon;
        public Texture2D ProtectIcon;
        public Texture2D NothingIcon;
        public Texture2D AnyIcon;

        public override void RuleOnGUI(Rect rect, Vector3Int position, int neighbor)
        {
            switch (neighbor)
            {
                case WallRule.Neighbor.Portal:
                    GUI.DrawTexture(rect, PortalIcon);
                    return;
                case WallRule.Neighbor.Protect:
                    GUI.DrawTexture(rect, ProtectIcon);
                    return;
                case WallRule.Neighbor.Nothing:
                    GUI.DrawTexture(rect, NothingIcon);
                    return;
                case WallRule.Neighbor.Any:
                    GUI.DrawTexture(rect, AnyIcon);
                    return;
            }

            base.RuleOnGUI(rect, position, neighbor);
        }
    }
}
