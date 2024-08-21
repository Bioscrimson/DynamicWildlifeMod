using System.Collections.Generic;
using Verse;
using RimWorld.Planet;

namespace Dynamic_Wildlife
{
    public class DynamicWildlifeWorldComponent : WorldComponent
    {
        private HashSet<int> penalizedTiles = new HashSet<int>();
        private WorldLayer_PenalizedTiles worldLayer;

        public DynamicWildlifeWorldComponent(World world) : base(world)
        {
            // Initialization can happen here, but to add layers I must use a good method?
            worldLayer = new WorldLayer_PenalizedTiles();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref penalizedTiles, "penalizedTiles", LookMode.Value);
        }

        public HashSet<int> GetPenalizedTiles()
        {
            return penalizedTiles;
        }

        public void MarkTileAsPenalized(int tileID)
        {
            if (penalizedTiles.Add(tileID))
            {
                // Assuming some method or hook to refresh world rendering
                RefreshWorldLayer();
            }
        }

        public bool IsTilePenalized(int tileID)
        {
            return penalizedTiles.Contains(tileID);
        }

        public void ClearPenalizedTilesOverlay()
        {
            penalizedTiles.Clear();
            RefreshWorldLayer();
        }

        private void RefreshWorldLayer()
        {
            // Ensure you refresh or re-render the custom layer correctly.
            Find.World.renderer.RegenerateAllLayers(); // Example method, I must find a good one
        }
    }
}
