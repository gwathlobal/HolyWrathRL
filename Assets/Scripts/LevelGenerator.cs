using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LevelGeneratorResult
{
    public List<BuildingLayoutResult> buildingLayoutResults;
}

public static class LevelGenerator {

    public struct BuildingPlacement
    {
        public BuildingLayoutEnum buildType;
        public int x;
        public int y;
    }

	public static LevelGeneratorResult GenerateLevel(Level level, LevelLayout levelLayout)
    {
        LevelGeneratorResult generatorResult = new LevelGeneratorResult()
        {
            buildingLayoutResults = new List<BuildingLayoutResult>()
        };

        List<BuildingPlacement> buildingsOnLevel = new List<BuildingPlacement>();

        int maxXres = (int)(level.maxX / BuildingLayout.GRID_SIZE);
        int maxYres = (int)(level.maxY / BuildingLayout.GRID_SIZE);

        // initialize reserved buildings array with free cells
        BuildingLayoutType[,] reservedBuildings = new BuildingLayoutType[maxXres, maxYres];
        for (int gy = 0; gy < maxYres; gy++)
        {
            for (int gx = 0; gx < maxXres; gx++)
            {
                reservedBuildings[gx, gy] = BuildingLayoutType.buildFree;
            }
        }

        // make border cells empty
        for (int gx = 0; gx < maxXres; gx++)
        {
            reservedBuildings[gx, 0] = BuildingLayoutType.buildEmpty;
            reservedBuildings[gx, maxYres - 1] = BuildingLayoutType.buildEmpty;
        }

        for (int gy = 0; gy < maxYres; gy++)
        {
            reservedBuildings[0, gy] = BuildingLayoutType.buildEmpty;
            reservedBuildings[maxXres - 1, gy] = BuildingLayoutType.buildEmpty;
        }

        // place buildings from pre-processed function
        if (levelLayout.PreProcessFunc != null)
            buildingsOnLevel.AddRange(levelLayout.PreProcessFunc(levelLayout, level, reservedBuildings));

        // make reservations for random buildings
        for (int gy = 0; gy < maxYres; gy++)
        {
            for (int gx = 0; gx < maxXres; gx++)
            {
                // make a list of buildings 
                List<BuildingLayoutEnum> availBuildings = new List<BuildingLayoutEnum>();
                foreach (BuildingLayoutType bt in levelLayout.buildingLayouts)
                {
                    foreach (BuildingLayout bl in BuildingLayouts.buildLayouts.Values)
                    {
                        if (bl.buildType == bt) availBuildings.Add(bl.id);
                    }
                }

                if (availBuildings.Count > 0)
                {
                    do
                    {
                        // pick a building 
                        BuildingLayoutEnum chosenBuilding = availBuildings[Random.Range(0, availBuildings.Count)];
                        bool buildingFit = true;

                        // try to fit it in
                        BuildingLayout bl = BuildingLayouts.buildLayouts[chosenBuilding];
                        for (int x = 0; x < bl.gw; x++)
                        {
                            for (int y = 0; y < bl.gh; y++)
                            {
                                if (gx + x >= maxXres || gy + y >= maxYres || reservedBuildings[gx + x, gy + y] != BuildingLayoutType.buildFree)
                                    buildingFit = false;
                            }
                        }

                        // if not, try next building
                        if (buildingFit)
                        {
                            buildingsOnLevel.Add(new BuildingPlacement()
                            {
                                buildType = bl.id,
                                x = gx,
                                y = gy
                            });
                            for (int x = 0; x < bl.gw; x++)
                            {
                                for (int y = 0; y < bl.gh; y++)
                                {
                                    reservedBuildings[gx + x, gy + y] = bl.buildType;
                                }
                            }
                            break;
                        }
                        else
                        {
                            availBuildings.Remove(chosenBuilding);
                        }
                        
                    } while (availBuildings.Count > 0);
                }
            }
        }

        // place all buildings to the level
        foreach (BuildingPlacement bp in buildingsOnLevel)
        {
            BuildingLayout bl = BuildingLayouts.buildLayouts[bp.buildType];
            int gw = bl.gw;
            int gh = bl.gh;
            int lw = bl.lw;
            int lh = bl.lh;

            int fx = bp.x * BuildingLayout.GRID_SIZE + UnityEngine.Random.Range(0, gw * BuildingLayout.GRID_SIZE - lw + 1);
            int fy = bp.y * BuildingLayout.GRID_SIZE + UnityEngine.Random.Range(0, gh * BuildingLayout.GRID_SIZE - lh + 1);

            BuildingLayoutResult buildResult = bl.PlaceBuilding(level, levelLayout, fx, fy);
            buildResult.sx = fx;
            buildResult.sy = fy;

            generatorResult.buildingLayoutResults.Add(buildResult);

        }

        // placing borders
        for (int x = 0; x < level.maxX; x++)
        {
            if (level.terrain[x, 0] == TerrainTypeEnum.terrainWaterTar)
                level.terrain[x, 0] = TerrainTypeEnum.terrainWaterTarBorder;
            else 
                level.terrain[x, 0] = levelLayout.terrainBorder;

            if (level.terrain[x, level.maxY - 1] == TerrainTypeEnum.terrainWaterTar)
                level.terrain[x, level.maxY - 1] = TerrainTypeEnum.terrainWaterTarBorder;
            else
                level.terrain[x, level.maxY - 1] = levelLayout.terrainBorder;
        }

        for (int y = 0; y < level.maxY; y++)
        {
            if (level.terrain[0, y] == TerrainTypeEnum.terrainWaterTar)
                level.terrain[0, y] = TerrainTypeEnum.terrainWaterTarBorder;
            else
                level.terrain[0, y] = levelLayout.terrainBorder;

            if (level.terrain[level.maxX - 1, y] == TerrainTypeEnum.terrainWaterTar)
                level.terrain[level.maxX - 1, y] = TerrainTypeEnum.terrainWaterTarBorder;
            else
                level.terrain[level.maxX - 1, y] = levelLayout.terrainBorder;
        }

        return generatorResult;
    }

}
