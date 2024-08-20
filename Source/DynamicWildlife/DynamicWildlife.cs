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

            // Additional initialization logic if needed
            Log.Message("DynamicWildlifeMod initialized and Harmony patches applied.");
        }
    }
}