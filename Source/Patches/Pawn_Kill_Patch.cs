using HarmonyLib;
using Verse;

namespace Dynamic_Wildlife.Patches
{
    [HarmonyPatch(typeof(Pawn), "Kill")]
    public static class Pawn_Kill_Patch
    {
        static void Postfix(Pawn __instance, DamageInfo? dinfo, Hediff exactCulprit)
        {
            var map = __instance.prevMap;

            if (map == null)
            {
                Log.Warning($"Pawn's map is null. Pawn: {__instance.LabelShort}, Kind: {__instance.kindDef.defName}");
                return;
            }

            Log.Message($"Kill Postfix called for pawn: {__instance.LabelShort}, map: {map}");

            if (__instance.RaceProps.Animal)
            {
                var mapComponent = map.GetComponent<DynamicWildlifeMapComponent>();
                if (mapComponent != null)
                {
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
