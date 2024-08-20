using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Dynamic_Wildlife
{
    public class DynamicWildlifeMapComponent : MapComponent
    {
        private Dictionary<string, float> adjustedCommonality = new Dictionary<string, float>();
        private const float PenaltyPerDeath = 0.01f;
        private bool initialized = false;

        public DynamicWildlifeMapComponent(Map map) : base(map)
        {
            Log.Message("DynamicWildlifeMapComponent initialized.");
        }

        // Override ExposeData to save and load adjusted commonality
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref adjustedCommonality, "adjustedCommonality", LookMode.Value, LookMode.Value);

            // Log to verify the values during save/load
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                Log.Message("Saving adjusted commonality values:");
                foreach (var kvp in adjustedCommonality)
                {
                    Log.Message($"- {kvp.Key}: {kvp.Value:F2}");
                }
            }
            else if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                Log.Message("Loading adjusted commonality values:");
                foreach (var kvp in adjustedCommonality)
                {
                    Log.Message($"- {kvp.Key}: {kvp.Value:F2}");
                }
            }
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();

            if (!initialized)
            {
                InitializeAnimalCommonalitiesForTile(map.Tile);
                initialized = true;
            }
        }

        private void InitializeAnimalCommonalitiesForTile(int tileID)
        {
            var commonalities = adjustedCommonality;

            foreach (var pawnKindDef in DefDatabase<PawnKindDef>.AllDefsListForReading)
            {
                if (pawnKindDef.RaceProps.Animal)
                {
                    string defName = pawnKindDef.defName;
                    if (!commonalities.ContainsKey(defName))
                    {
                        float baseCommonality = map.Biome.CommonalityOfAnimal(pawnKindDef);
                        commonalities[defName] = baseCommonality;
                    }
                }
            }
        }

        public void RecordAnimalDeath(string animalType)
        {
            if (adjustedCommonality.ContainsKey(animalType))
            {
                float currentCommonality = adjustedCommonality[animalType];
                float newCommonality = Mathf.Max(0f, currentCommonality - PenaltyPerDeath);
                adjustedCommonality[animalType] = newCommonality;
                Log.Message($"Animal death recorded: {animalType}, old commonality = {currentCommonality:F2}, new adjusted commonality = {newCommonality:F2}");
            }
            else
            {
                Log.Warning($"Attempted to record death for unknown animal type: {animalType}");
            }
        }

        public float GetAdjustedCommonality(string animalType)
        {
            return adjustedCommonality.TryGetValue(animalType, out float commonality) ? commonality : 0f;
        }
    }
}
