using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Dynamic_Wildlife
{
    public class AnimalSpawnAdjuster
    {
        public static AnimalSpawnAdjuster Instance { get; } = new AnimalSpawnAdjuster();

        private Dictionary<string, float> adjustedCommonality = new Dictionary<string, float>();

        // Method to record animal deaths and adjust commonality
        public void RecordAnimalDeath(string animalType)
        {
            if (adjustedCommonality.ContainsKey(animalType))
            {
                adjustedCommonality[animalType] = Mathf.Max(0f, adjustedCommonality[animalType] - 0.01f);
            }
            else
            {
                float baseCommonality = GetBaseCommonality(animalType);
                adjustedCommonality[animalType] = Mathf.Max(0f, baseCommonality - 0.01f);
            }
        }

        public float GetAdjustedCommonality(string animalType)
        {
            return adjustedCommonality.TryGetValue(animalType, out float value) ? value : GetBaseCommonality(animalType);
        }

        private float GetBaseCommonality(string animalType)
        {
            var pawnKindDef = DefDatabase<PawnKindDef>.GetNamedSilentFail(animalType);
            return pawnKindDef?.wildGroupSize.Average ?? 1f;
        }
    }
}
