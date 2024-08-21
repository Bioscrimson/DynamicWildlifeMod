using System.Collections.Generic;
using Verse;
using RimWorld.Planet;

namespace Dynamic_Wildlife
{
    public class DynamicWildlifeWorldComponent : WorldComponent
    {
        public HashSet<int> PenalizedTiles => penalizedTiles; // getter
        private HashSet<int> penalizedTiles = new HashSet<int>(); // can't wrap this into a single property due to needing to save in ExposeData()
        
        public DynamicWildlifeWorldComponent(World world) : base(world)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref penalizedTiles, "penalizedTiles", LookMode.Value);
        }

        public void MarkTileAsPenalized(int tileID)
        {
            if (penalizedTiles.Add(tileID))
            {
                world.renderer.SetDirty<WorldLayer_PenalizedTiles>();
            }
        }

        //May be obsolete due to a public getter allowing for contains to be applied at location of use
        public bool IsTilePenalized(int tileID)
        {
            return penalizedTiles.Contains(tileID);
        }
    }
}
