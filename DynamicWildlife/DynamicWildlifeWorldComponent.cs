using System.Collections.Generic;
using Verse;
using RimWorld.Planet;
using UnityEngine;

namespace Dynamic_Wildlife
{
    public class DynamicWildlifeWorldComponent : WorldComponent
    {
        private HashSet<int> penalizedTiles = new HashSet<int>();
        private WorldLayer_PenalizedTiles worldLayer;

        public DynamicWildlifeWorldComponent(World world) : base(world)
        {
            // Initialize worldLayer
            worldLayer = new WorldLayer_PenalizedTiles();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref penalizedTiles, "penalizedTiles", LookMode.Value);
        }

        public void MarkTileAsPenalized(int tileID)
        {
            penalizedTiles.Add(tileID);
            DrawPenalizedTilesOverlay(); // Update the overlay
        }

        public bool IsTilePenalized(int tileID)
        {
            return penalizedTiles.Contains(tileID);
        }

        public void ClearPenalizedTilesOverlay()
        {
            penalizedTiles.Clear();
            DrawPenalizedTilesOverlay(); // Update the overlay
        }

        public HashSet<int> GetPenalizedTiles()
        {
            return penalizedTiles;
        }

        public void DrawPenalizedTilesOverlay()
        {
            // Ensure worldLayer is initialized
            if (worldLayer == null)
            {
                worldLayer = new WorldLayer_PenalizedTiles();
            }

            // Add or update the world layer
            Find.World.renderer.wantedLayers.Add(worldLayer); // This assumes I want to update the layers list directly

            // Alternatively, force the world to redraw
            Find.World.renderer.RebuildAllLayers(); // Check if this method exists or use similar method to trigger a redraw
        }
    }
}
