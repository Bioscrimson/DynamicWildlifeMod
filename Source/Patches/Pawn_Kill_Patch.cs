using HarmonyLib;
using Verse;

namespace Dynamic_Wildlife.Patches
{
    [HarmonyPatch(typeof(Pawn), "Kill")]
    public static class Pawn_Kill_Patch
    {
        static void Postfix(Pawn __instance, DamageInfo? dinfo, Hediff exactCulprit)
        {
            // Check if the pawn's map is available
            if (__instance.Map == null)
            {
                Log.Warning($"Pawn's map is null. Pawn: {__instance.LabelShort}, Kind: {__instance.kindDef.defName}");
                return; // Exit early if no map is available
            }

            // Log message to ensure this method is being called
            Log.Message($"Kill Postfix called for pawn: {__instance.LabelShort}, map: {__instance.Map}");

            // Ensure that the pawn is an animal and has died
            if (__instance.RaceProps.Animal)
            {
                var map = __instance.Map;
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
        }
    }
}
