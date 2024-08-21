using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Dynamic_Wildlife
{
    public class WorldTabUI : WorldComponent
    {
        private bool overlayEnabled = true;

        public WorldTabUI(World world) : base(world) { }

        public override void WorldComponentUpdate()
        {
            if (Find.World.renderer.wantedMode == WorldRenderMode.Planet)
            {
                DrawToggleButton();
            }
        }

        private void DrawToggleButton()
        {
            // Draw the button using Verse.Widgets
            Rect buttonRect = new Rect(10, 10, 150, 30);
            if (Widgets.ButtonText(buttonRect, overlayEnabled ? "Hide Animal Overlay" : "Show Animal Overlay"))
            {
                overlayEnabled = !overlayEnabled;
                var component = Find.World.GetComponent<DynamicWildlifeWorldComponent>();
                if (component != null)
                {
                    if (overlayEnabled)
                    {
                        component.DrawPenalizedTilesOverlay();
                    }
                    else
                    {
                        // Implement hiding or clearing of the overlay
                        component.ClearPenalizedTilesOverlay(); // Ensure this method exists or create it
                    }
                }
            }
        }
    }
}
