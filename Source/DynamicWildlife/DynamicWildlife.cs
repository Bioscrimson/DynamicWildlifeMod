using HarmonyLib;
using Verse;
using RimWorld.Planet;

namespace Dynamic_Wildlife
{
    public class DynamicWildlife : Mod
    {
        public DynamicWildlife(ModContentPack content) : base(content)
        {
            // Initialize Harmony
            Harmony harmony = new Harmony("com.dreadofcrimson.dynamicwildlife");
            harmony.PatchAll();

            // Register the WorldComponent and GameComponent
            LongEventHandler.ExecuteWhenFinished(() =>
            {
                // Ensure the DynamicWildlifeWorldComponent is added to the World
                if (Find.World != null && Find.World.GetComponent<DynamicWildlifeWorldComponent>() == null)
                {
                    Find.World.components.Add(new DynamicWildlifeWorldComponent(Find.World));
                }

                // Ensure the WorldTabUI component is added
                if (Current.Game != null)
                {
                    var existingComponent = Current.Game.GetComponent<WorldTabUI>();
                    if (existingComponent == null)
                    {
                        // Add WorldTabUI if it does not exist
                        Current.Game.components.Add(new WorldTabUI(Current.Game));
                    }
                }
            });

            // Log the initialization
            Log.Message("DynamicWildlifeMod initialized and Harmony patches applied.");
        }
    }
}
