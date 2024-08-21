using HarmonyLib;
using Verse;

namespace Dynamic_Wildlife
{
    public class DynamicWildlife : Mod
    {
        public DynamicWildlife(ModContentPack content) : base(content)
        {
            // Initialize Harmony
            Harmony harmony = new Harmony("com.dreadofcrimson.dynamicwildlife");
            harmony.PatchAll();

            // Register the GameComponent
            LongEventHandler.QueueLongEvent(() =>
            {
                if (Find.World != null && Find.World.GetComponent<DynamicWildlifeWorldComponent>() == null)
                {
                    Find.World.components.Add(new DynamicWildlifeWorldComponent(Find.World));
                }
            }, "Registering Dynamic Wildlife Components", false, null);

            // Log the initialization
            Log.Message("DynamicWildlifeMod initialized and Harmony patches applied.");
        }
    }
}
