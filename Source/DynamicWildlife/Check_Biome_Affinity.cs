using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using Verse;

namespace Dynamic_Wildlife
{
    public static class Check_Biome_Affinity
    {
        // Get the wildBiomes field from RaceProps using reflection
        private static FieldInfo GetWildBiomesField()
        {
            var racePropsType = typeof(RaceProperties);
            var field = racePropsType.GetField("wildBiomes", BindingFlags.NonPublic | BindingFlags.Instance);
            return field;
        }

        public static List<KeyValuePair<BiomeDef, float>> GetBiomeSuitability(PawnKindDef pawnKindDef)
        {
            if (pawnKindDef == null)
            {
                Log.Warning("PawnKindDef is null.");
                return null;
            }

            var raceProps = pawnKindDef.race;
            if (raceProps == null)
            {
                Log.Warning($"{pawnKindDef.defName} has no RaceProperties.");
                return null;
            }

            var wildBiomesField = GetWildBiomesField();
            if (wildBiomesField == null)
            {
                Log.Warning("Unable to find wildBiomes field.");
                return null;
            }

            var wildBiomes = wildBiomesField.GetValue(raceProps) as List<AnimalBiomeRecord>;
            if (wildBiomes == null || !wildBiomes.Any())
            {
                Log.Warning($"{pawnKindDef.defName} does not have any associated wild biomes.");
                return null;
            }

            // Convert List<AnimalBiomeRecord> to List<KeyValuePair<BiomeDef, float>>
            var sortedBiomes = wildBiomes
                .Select(record => new KeyValuePair<BiomeDef, float>(record.biome, record.commonality))
                .OrderByDescending(biomeEntry => biomeEntry.Value)
                .ToList();

            return sortedBiomes;
        }

        public static void LogBiomeAffinitiesForAllAnimals()
        {
            var animalPawnKindDefs = DefDatabase<PawnKindDef>.AllDefsListForReading
                .Where(def => def.RaceProps.Animal);

            if (!animalPawnKindDefs.Any())
            {
                Log.Warning("No animal PawnKindDefs found.");
                return;
            }

            foreach (var pawnKindDef in animalPawnKindDefs)
            {
                Log.Message($"Biome suitability for {pawnKindDef.defName}:");
                var sortedBiomes = GetBiomeSuitability(pawnKindDef);
                if (sortedBiomes != null)
                {
                    foreach (var biomeEntry in sortedBiomes)
                    {
                        Log.Message($"- Biome: {biomeEntry.Key.label}, Commonality: {biomeEntry.Value:F2}");
                    }
                }
                Log.Message("---------------------------");
            }
        }
    }
}
