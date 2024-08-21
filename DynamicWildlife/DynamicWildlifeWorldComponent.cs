using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld.Planet;

namespace Dynamic_Wildlife
{
    public class DynamicWildlifeWorldComponent : WorldComponent
    {
        private HashSet<int> penalizedTiles = new HashSet<int>();
        private Material penalizedTileMaterial;

        public DynamicWildlifeWorldComponent(World world) : base(world)
        {
            // Queue initialization to run on the main thread
            LongEventHandler.QueueLongEvent(InitializeMaterial, "LoadingDynamicWildlifeMaterial", false, null);
        }

        private void InitializeMaterial()
        {
            // Initialize the material with the new texture path
            penalizedTileMaterial = MaterialPool.MatFrom("UI/Overlays/planetglow", ShaderDatabase.WorldOverlayTransparent, Color.red);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref penalizedTiles, "penalizedTiles", LookMode.Value);
        }

        public void MarkTileAsPenalized(int tileID)
        {
            penalizedTiles.Add(tileID);
        }

        public bool IsTilePenalized(int tileID)
        {
            return penalizedTiles.Contains(tileID);
        }

        public void DrawPenalizedTilesOverlay()
        {
            if (penalizedTileMaterial == null)
            {
                // Material not initialized yet
                return;
            }

            foreach (int tileID in penalizedTiles)
            {
                Vector3 tileCenter = Find.WorldGrid.GetTileCenter(tileID);
                WorldRendererUtility.DrawQuadTangentialToPlanet(tileCenter, 0.4f, 0.01f, penalizedTileMaterial);
            }
        }

        public void ClearPenalizedTilesOverlay()
        {
            // Clear the overlay
            penalizedTiles.Clear(); // Clear the tiles
        }
    }
}
