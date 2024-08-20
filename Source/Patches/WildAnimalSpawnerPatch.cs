using HarmonyLib;
using RimWorld;
using Verse;

namespace Dynamic_Wildlife.Patches
{
    [HarmonyPatch(typeof(WildAnimalSpawner), "CommonalityOfAnimalNow")]
    public static class WildAnimalSpawnerPatch
    {
        static void Postfix(ref float __result, PawnKindDef def)
        {
            var map = Find.CurrentMap;
            if (map == null) return;

            var mapComponent = map.GetComponent<DynamicWildlifeMapComponent>();
            if (mapComponent == null) return;

            // Get the adjusted commonality for the animal type on this map
            float adjustedCommonality = mapComponent.GetAdjustedCommonality(def.defName);

            // Apply the adjusted commonality to the spawn rate
            __result *= adjustedCommonality;
        }
    }
}
