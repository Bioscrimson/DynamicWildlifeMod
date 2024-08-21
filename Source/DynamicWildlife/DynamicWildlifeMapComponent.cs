using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace Dynamic_Wildlife
{
    public class DynamicWildlifeMapComponent : MapComponent
    {
        private Dictionary<string, float> adjustedCommonality = new Dictionary<string, float>();
        private const float PenaltyPerDeath = 0.01f;
        private const float NeighborPenaltyMultiplier = 0.5f; // 50% penalty for neighboring tiles
        private bool initialized = false;

        public DynamicWildlifeMapComponent(Map map) : base(map)
        {
            Log.Message("DynamicWildlifeMapComponent initialized.");
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref adjustedCommonality, "adjustedCommonality", LookMode.Value, LookMode.Value);

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

                if (newCommonality <= 0f)
                {
                    ApplyPenaltyToNeighboringTiles(animalType);
                }
            }
            else
            {
                Log.Warning($"Attempted to record death for unknown animal type: {animalType}");
            }
        }

        private void ApplyPenaltyToNeighboringTiles(string animalType)
        {
            int tileID = map.Tile;
            List<int> neighbors = new List<int>();
            Find.WorldGrid.GetTileNeighbors(tileID, neighbors);

            foreach (int neighborTileID in neighbors)
            {
                foreach (Map neighborMap in Find.Maps)
                {
                    if (neighborMap.Tile == neighborTileID)
                    {
                        DynamicWildlifeMapComponent neighborComponent = neighborMap.GetComponent<DynamicWildlifeMapComponent>();
                        if (neighborComponent != null)
                        {
                            neighborComponent.ApplyNeighborPenalty(animalType, NeighborPenaltyMultiplier);
                        }
                    }
                }
            }

            Log.Message($"Applied neighbor penalty to {neighbors.Count} neighboring tiles for animal type {animalType}.");
        }

        public void ApplyNeighborPenalty(string animalType, float penaltyMultiplier)
        {
            if (adjustedCommonality.ContainsKey(animalType))
            {
                float currentCommonality = adjustedCommonality[animalType];
                float penalty = currentCommonality * penaltyMultiplier;
                float newCommonality = Mathf.Max(0f, currentCommonality - penalty);
                adjustedCommonality[animalType] = newCommonality;

                Log.Message($"Neighbor penalty applied to {animalType}: old commonality = {currentCommonality:F2}, penalty = {penalty:F2}, new adjusted commonality = {newCommonality:F2}");

                // Mark the current map's tile as penalized
                Find.World.GetComponent<DynamicWildlifeWorldComponent>().MarkTileAsPenalized(map.Tile);
            }
        }


        public float GetAdjustedCommonality(string animalType)
        {
            return adjustedCommonality.TryGetValue(animalType, out float commonality) ? commonality : 0f;
        }
    }
}
