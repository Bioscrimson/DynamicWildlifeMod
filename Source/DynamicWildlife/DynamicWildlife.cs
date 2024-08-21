using HarmonyLib;
using Verse;
using RimWorld.Planet;
using UnityEngine;

namespace Dynamic_Wildlife
{
    public class DynamicWildlife : Mod
    {
        public DynamicWildlife(ModContentPack content) : base(content)
        {
            // Initialize Harmony
            Harmony harmony = new Harmony("com.dreadofcrimson.dynamicwildlife");
            harmony.PatchAll();

            // Log the initialization
            Log.Message("DynamicWildlifeMod initialized and Harmony patches applied.");
        }
    }
}
