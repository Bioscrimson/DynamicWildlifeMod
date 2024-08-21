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

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref adjustedCommonality, "adjustedCommonality", LookMode.Value, LookMode.Value);

            // Log a single message to verify that the adjustedCommonality dictionary is being saved/loaded
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                Log.Message("Saving adjusted commonality values for all animals.");
            }
            else if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                Log.Message("Loading adjusted commonality values for all animals.");
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
            int initializedCount = 0;

            foreach (var pawnKindDef in DefDatabase<PawnKindDef>.AllDefsListForReading)
            {
                if (pawnKindDef.RaceProps.Animal)
                {
                    string defName = pawnKindDef.defName;
                    if (!adjustedCommonality.ContainsKey(defName))
                    {
                        float baseCommonality = map.Biome.CommonalityOfAnimal(pawnKindDef);
                        adjustedCommonality[defName] = baseCommonality;
                        initializedCount++;
                    }
                }
            }

            if (initializedCount > 0)
            {
                Log.Message($"Initialized base commonality values for {initializedCount} animals.");
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
