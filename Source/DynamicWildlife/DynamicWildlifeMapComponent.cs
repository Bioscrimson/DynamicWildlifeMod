using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Dynamic_Wildlife
{
    public class DynamicWildlifeMapComponent : MapComponent
    {
        private Dictionary<int, Dictionary<string, float>> tileAnimalCommonalities = new Dictionary<int, Dictionary<string, float>>();
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
            if (!tileAnimalCommonalities.ContainsKey(tileID))
            {
                tileAnimalCommonalities[tileID] = new Dictionary<string, float>();
            }

            var commonalities = tileAnimalCommonalities[tileID];

            foreach (var pawnKindDef in DefDatabase<PawnKindDef>.AllDefsListForReading)
            {
                if (pawnKindDef.RaceProps.Animal)
                {
                    string defName = pawnKindDef.defName;
                    if (!commonalities.ContainsKey(defName))
                    {
                        float baseCommonality = map.Biome.CommonalityOfAnimal(pawnKindDef);
                        commonalities[defName] = baseCommonality;
                        adjustedCommonality[defName] = baseCommonality;
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
