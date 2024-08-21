using Verse;
using RimWorld;
using System.Linq;
using LudeonTK;

namespace Dynamic_Wildlife
{
    public class DebugCommands
    {
        [DebugAction("Dynamic Wildlife", "Get Animal Commonality", allowedGameStates = AllowedGameStates.Playing)]
        public static void GetAnimalCommonality()
        {
            var map = Find.CurrentMap;
            if (map == null)
            {
                Log.Warning("Current map is null. Cannot get animal commonality.");
                return;
            }

            var mapComponent = map.GetComponent<DynamicWildlifeMapComponent>();
            if (mapComponent == null)
            {
                Log.Warning("DynamicWildlifeMapComponent is not present on the map.");
                return;
            }

            var animalPawnKindDefs = DefDatabase<PawnKindDef>.AllDefsListForReading
                .Where(def => def.RaceProps.Animal);

            if (!animalPawnKindDefs.Any())
            {
                Log.Warning("No animal PawnKindDefs found.");
                return;
            }

            Log.Message("Available animal PawnKindDefs:");

            foreach (var pawnKindDef in animalPawnKindDefs)
            {
                string animalType = pawnKindDef.defName;

                float baseCommonality = map.Biome.CommonalityOfAnimal(pawnKindDef);
                float adjustedCommonality = mapComponent.GetAdjustedCommonality(animalType);

                Log.Message($"- {animalType}: Base commonality = {baseCommonality:F2}, Adjusted commonality = {adjustedCommonality:F2}");
            }
        }
    }
}
