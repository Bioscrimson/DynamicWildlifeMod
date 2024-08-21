using UnityEngine;
using Verse;

namespace Dynamic_Wildlife
{
    public class WorldTabUI : GameComponent
    {
        private bool overlayEnabled = true;
        private DynamicWildlifeWorldComponent wildlifeWorldComponent;

        public WorldTabUI(Game game) : base()
        {
            // Initialization code
            if (Find.World != null)
            {
                wildlifeWorldComponent = Find.World.GetComponent<DynamicWildlifeWorldComponent>();
            }
        }

        public override void GameComponentOnGUI()
        {
            // Draw the button in the world view
            DrawToggleButton();
        }

        private void DrawToggleButton()
        {
            Rect buttonRect = new Rect(10, 10, 150, 30);
            if (Widgets.ButtonText(buttonRect, overlayEnabled ? "Hide Animal Overlay" : "Show Animal Overlay"))
            {
                overlayEnabled = !overlayEnabled;
                if (wildlifeWorldComponent != null)
                {
                    if (overlayEnabled)
                    {
                        wildlifeWorldComponent.DrawPenalizedTilesOverlay();
                    }
                    else
                    {
                        wildlifeWorldComponent.ClearPenalizedTilesOverlay();
                    }
                }
            }
        }
    }
}
