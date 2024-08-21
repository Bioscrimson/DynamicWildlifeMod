using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld.Planet;

namespace Dynamic_Wildlife
{
    [StaticConstructorOnStartup]
    public class WorldLayer_PenalizedTiles : WorldLayer
    {
        //This is set in the main thread (required) during the StaticConstructorOnStartup stage
        private static Material PenalizedTileMat = MaterialPool.MatFrom("UI/Overlays/PlanetGlow", ShaderDatabase.WorldOverlayTransparent, Color.red);
        private DynamicWildlifeWorldComponent worldComponent;
        
        public WorldLayer_PenalizedTiles()
        {
        }

        public override IEnumerable Regenerate()
        {
            if(worldComponent is null) worldComponent = Find.World.GetComponent<DynamicWildlifeWorldComponent>(); //WorldComponents are instantiated _after_ WorldLayers, so cache on first regen (which is after they are both created) 
            
            foreach(var item in base.Regenerate()) // even though base.Regeneration only yields once, better to encapsulate with an iteration block for safety
            {
                yield return item;
            }

            LayerSubMesh subMesh = GetSubMesh(PenalizedTileMat);

            foreach (int tileID in worldComponent.PenalizedTiles)
            {
                Vector3 tileCenter = Find.WorldGrid.GetTileCenter(tileID);
                WorldRendererUtility.PrintQuadTangentialToPlanet(tileCenter, 0.7f * Find.WorldGrid.averageTileSize, 0.01f, subMesh, false, true, true);
            }

            FinalizeMesh(MeshParts.All);
        }

        public override void Render()
        {
            if (!WorldTabUI.OverlayEnabled) return; //skip render when UI toggled
            base.Render();
        }
    }
}
