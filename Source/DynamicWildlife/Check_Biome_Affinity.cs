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
        private static FieldInfo GetWildBiomesField()
        {
            // Try getting the field with both possible names and using different flags
            var type = typeof(PawnKindDef);
            FieldInfo field = type.GetField("wildBiomes", BindingFlags.NonPublic | BindingFlags.Instance);

            if (field == null)
            {
                // Check for potential alternative field names or protected/internal access
                Log.Warning("Field 'wildBiomes' not found. Checking other possible names.");
                // You may need to list all fields to find the correct one
                var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var f in fields)
                {
                    Log.Message($"Field found: {f.Name}");
                }
            }

            return field;
        }

        public static List<KeyValuePair<BiomeDef, float>> GetBiomeSuitability(PawnKindDef pawnKindDef)
        {
            if (pawnKindDef == null)
            {
                Log.Warning("PawnKindDef is null.");
                return null;
            }

            var wildBiomesField = GetWildBiomesField();
            if (wildBiomesField == null)
            {
                Log.Warning("Unable to find wildBiomes field.");
                return null;
            }

            var wildBiomes = wildBiomesField.GetValue(pawnKindDef) as List<AnimalBiomeRecord>;
            if (wildBiomes == null || !wildBiomes.Any())
            {
                Log.Warning($"{pawnKindDef.defName} does not have any associated wild biomes.");
                return null;
            }

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
