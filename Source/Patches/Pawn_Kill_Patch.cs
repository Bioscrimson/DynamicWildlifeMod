using HarmonyLib;
using Verse;

namespace Dynamic_Wildlife.Patches
{
    [HarmonyPatch(typeof(Pawn), "Kill")]
    public static class Pawn_Kill_Patch
    {
        static bool Prefix(Pawn __instance, ref DamageInfo? dinfo, ref Hediff exactCulprit)
        {
            // Check if the map is available before the pawn is killed
            var map = __instance.Map ?? __instance.prevMap;

            if (map == null)
            {
                Log.Warning($"Unable to find the map for pawn: {__instance.LabelShort}, Kind: {__instance.kindDef.defName}");
                return true; // Allow the original Kill method to proceed
            }

            // Ensure that the pawn is an animal and has died
            if (__instance.RaceProps.Animal)
            {
                var mapComponent = map.GetComponent<DynamicWildlifeMapComponent>();
                if (mapComponent != null)
                {
                    // Record the animal's death
                    mapComponent.RecordAnimalDeath(__instance.kindDef.defName);
                }
                else
                {
                    Log.Warning("DynamicWildlifeMapComponent is not present on the map.");
                }
            }

            // Allow the original Kill method to proceed
            return true;
        }
    }
}
