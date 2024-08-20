using HarmonyLib;
using Verse;
using System.Linq;

namespace Dynamic_Wildlife.Patches
{
    [HarmonyPatch(typeof(Pawn), "Kill")]
    public static class Pawn_Kill_Patch
    {
        static void Postfix(Pawn __instance, DamageInfo? dinfo, Hediff exactCulprit)
        {
            // Get the map from the pawn
            var map = __instance.Map ?? Find.Maps.FirstOrDefault(m => m.mapPawns.AllPawnsSpawned.Any(p => p == __instance));

            if (map == null)
            {
                Log.Warning($"Unable to find the map for pawn: {__instance.LabelShort}, Kind: {__instance.kindDef.defName}");
                return; // Exit early if no map is available
            }

            // Log message to ensure this method is being called
            Log.Message($"Kill Postfix called for pawn: {__instance.LabelShort}, map: {map}");

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
        }
    }
}
