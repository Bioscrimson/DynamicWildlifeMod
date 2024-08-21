using UnityEngine;
using Verse;

namespace Dynamic_Wildlife
{
    public class WorldTabUI : GameComponent
    {
        public static bool OverlayEnabled { get; private set;} = false;
        
        public WorldTabUI(Game game) : base()
        {
        }

        public override void GameComponentOnGUI()
        {
            if (!WorldRendererUtility.WorldRenderedNow) return;
            // Draw the button in the world view
            DrawToggleButton();
        }

        private void DrawToggleButton()
        {
            var buttonRect = new Rect(10, 10, 150, 30);
            if (Widgets.ButtonText(buttonRect, OverlayEnabled ? "Hide Animal Overlay" : "Show Animal Overlay"))
            {
                OverlayEnabled = !OverlayEnabled;
            }
        }
    }
}
