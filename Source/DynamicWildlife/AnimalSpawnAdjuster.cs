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
            // Check if the animal type already has an adjusted commonality
            if (adjustedCommonality.ContainsKey(animalType))
            {
                // Decrease the adjusted commonality, ensuring it does not go below zero
                adjustedCommonality[animalType] = Mathf.Max(0f, adjustedCommonality[animalType] - 0.01f);
            }
            else
            {
                // Fetch the base commonality and apply the penalty
                float baseCommonality = GetBaseCommonality(animalType);
                adjustedCommonality[animalType] = Mathf.Max(0f, baseCommonality - 0.01f);
            }
        }

        // Get the adjusted commonality for an animal type
        public float GetAdjustedCommonality(string animalType)
        {
            // Return the adjusted commonality if it exists, otherwise return the base commonality
            return adjustedCommonality.TryGetValue(animalType, out float value) ? value : GetBaseCommonality(animalType);
        }

        // Retrieve the base commonality for an animal type
        private float GetBaseCommonality(string animalType)
        {
            var pawnKindDef = DefDatabase<PawnKindDef>.GetNamedSilentFail(animalType);
            return pawnKindDef?.wildGroupSize.Average ?? 1f; // Default to 1f if animal type is not found
        }
    }
}
