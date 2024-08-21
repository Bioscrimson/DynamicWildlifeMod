using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld.Planet;

namespace Dynamic_Wildlife
{
    public class WorldLayer_PenalizedTiles : WorldLayer
    {
        private Material penalizedTileMaterial;

        public WorldLayer_PenalizedTiles()
        {
            penalizedTileMaterial = MaterialPool.MatFrom("World/PlanetGlow/planetglow", ShaderDatabase.WorldOverlayTransparent, Color.red);
        }

        public override IEnumerable Regenerate()
        {
            base.Regenerate();

            LayerSubMesh subMesh = GetSubMesh(penalizedTileMaterial);
            subMesh.Clear(MeshParts.All);

            // Get the DynamicWildlifeWorldComponent instance
            DynamicWildlifeWorldComponent dwComponent = Find.World.GetComponent<DynamicWildlifeWorldComponent>();
            if (dwComponent != null)
            {
                HashSet<int> penalizedTiles = dwComponent.GetPenalizedTiles();
                foreach (int tileID in penalizedTiles)
                {
                    Vector3 tileCenter = Find.WorldGrid.GetTileCenter(tileID);
                    WorldRendererUtility.PrintQuadTangentialToPlanet(tileCenter, 0.7f * Find.WorldGrid.averageTileSize, 0.01f, subMesh, false, true, true);
                }
            }

            FinalizeMesh(MeshParts.All);

            yield break; // Return an empty IEnumerable
        }
    }
}
