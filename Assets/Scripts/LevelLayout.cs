using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelLayoutEnum
{
    levelTest, levelDesolatePlains, levelTarRiver, levelSlimeForest, levelCityDistrict, levelAbandonedDistrict, levelCorruptedDistrict
}

public delegate List<LevelGenerator.BuildingPlacement> LevelPreProcessFunc(LevelLayout ll, Level level, BuildingLayoutType[,] reservedBuildings, MonsterLayoutEnum monsterLayout);
public delegate void LevelPostProcessFunc(LevelLayout ll, Level level);

public class LevelLayout {

    public LevelLayoutEnum layoutId;

    public string name;

    public int minLvl;

    public TerrainTypeEnum terrainBorder;
    public TerrainTypeEnum terrainFloorPrimary;
    public TerrainTypeEnum terrainFloorAlt;
    public TerrainTypeEnum terrainWall;
    public TerrainTypeEnum terrainGrass;
    public TerrainTypeEnum terrainGrassAlt;
    public TerrainTypeEnum terrainWater;
    public TerrainTypeEnum terrainTree;

    public List<BuildingLayoutType> buildingLayouts;
    public List<MonsterLayoutEnum> monsterLayouts;
    public List<LevelModifierTypes.LevelModifierEnum> levelModifiers;

    public LevelPreProcessFunc PreProcessFunc;
    public LevelPostProcessFunc PostProcessFunc;
}

public static class LevelLayouts
{
    public static Dictionary<LevelLayoutEnum, LevelLayout> levelLayouts;

    public static void InitializeLayouts()
    {
        levelLayouts = new Dictionary<LevelLayoutEnum, LevelLayout>();

        Add(LevelLayoutEnum.levelTest, "Test location", 0,
            TerrainTypeEnum.terrainWall, TerrainTypeEnum.terrainStoneFloor, TerrainTypeEnum.terrainStoneFloorBright, TerrainTypeEnum.terrainStoneWall,
            TerrainTypeEnum.terrainSlimeFloor, TerrainTypeEnum.terrainSlimeFloorBright, TerrainTypeEnum.terrainWaterTar, TerrainTypeEnum.terrainCorruptedTree,
            new List<BuildingLayoutType> { BuildingLayoutType.buildEmpty },
            new List<MonsterLayoutEnum>(),
            new List<LevelModifierTypes.LevelModifierEnum>(),
            null,
            null);

        Add(LevelLayoutEnum.levelDesolatePlains, "Desolate plains", 0,
            TerrainTypeEnum.terrainStoneFloorBorder, TerrainTypeEnum.terrainStoneFloor, TerrainTypeEnum.terrainStoneFloorBright, TerrainTypeEnum.terrainStoneWall,
            TerrainTypeEnum.terrainSlimeFloor, TerrainTypeEnum.terrainSlimeFloorBright, TerrainTypeEnum.terrainWaterTar, TerrainTypeEnum.terrainCorruptedTree,
            new List<BuildingLayoutType> { BuildingLayoutType.buildEmpty, BuildingLayoutType.buildShape, BuildingLayoutType.buildTarPool, BuildingLayoutType.buildSingleTree },
            new List<MonsterLayoutEnum>() { MonsterLayoutEnum.levelBeastsOnly, MonsterLayoutEnum.levelCrimsonDemons, MonsterLayoutEnum.levelMachineDemons,
                MonsterLayoutEnum.levelBeastsAndDemons },
            new List<LevelModifierTypes.LevelModifierEnum>() { LevelModifierTypes.LevelModifierEnum.LevModAngel },
            null,
            null);

        Add(LevelLayoutEnum.levelTarRiver, "Tar river", 0,
            TerrainTypeEnum.terrainStoneFloorBorder, TerrainTypeEnum.terrainStoneFloor, TerrainTypeEnum.terrainStoneFloorBright, TerrainTypeEnum.terrainStoneWall,
            TerrainTypeEnum.terrainSlimeFloor, TerrainTypeEnum.terrainSlimeFloorBright, TerrainTypeEnum.terrainWaterTar, TerrainTypeEnum.terrainCorruptedTree,
            new List<BuildingLayoutType> { BuildingLayoutType.buildEmpty, BuildingLayoutType.buildShape, BuildingLayoutType.buildSingleTree },
            new List<MonsterLayoutEnum>() { MonsterLayoutEnum.levelBeastsOnly, MonsterLayoutEnum.levelCrimsonDemons, MonsterLayoutEnum.levelMachineDemons,
                MonsterLayoutEnum.levelBeastsAndDemons },
            new List<LevelModifierTypes.LevelModifierEnum>() { LevelModifierTypes.LevelModifierEnum.LevModAngel },
            (LevelLayout ll, Level level, BuildingLayoutType[,] reservedBuildings, MonsterLayoutEnum monsterLayout) =>
            {
                int maxXres = (int)(level.maxX / BuildingLayout.GRID_SIZE);
                int maxYres = (int)(level.maxY / BuildingLayout.GRID_SIZE);


                int startX;
                int startY;
                BuildingLayout bl;

                if (Random.Range(0, 2) == 0)
                {
                    startX = maxXres / 2 - 1;
                    startY = 0;
                    bl = BuildingLayouts.buildLayouts[BuildingLayoutEnum.buildTarRiverV];
                }
                else
                {
                    startX = 0;
                    startY = maxYres / 2 - 1;
                    bl = BuildingLayouts.buildLayouts[BuildingLayoutEnum.buildTarRiverH];
                }
                List<LevelGenerator.BuildingPlacement> buildingsOnLevel = new List<LevelGenerator.BuildingPlacement>();

                buildingsOnLevel.Add(new LevelGenerator.BuildingPlacement()
                {
                    buildType = bl.id,
                    x = startX,
                    y = startY
                });
                for (int x = 0; x < bl.gw; x++)
                {
                    for (int y = 0; y < bl.gh; y++)
                    {
                        reservedBuildings[startX + x, startY + y] = bl.buildType;
                    }
                }
                return buildingsOnLevel;
            },
            null);

        Add(LevelLayoutEnum.levelSlimeForest, "Slime Forest", 0,
            TerrainTypeEnum.terrainSlimeFloorBorder, TerrainTypeEnum.terrainSlimeFloor, TerrainTypeEnum.terrainSlimeFloorBright, TerrainTypeEnum.terrainStoneWall,
            TerrainTypeEnum.terrainSlimeFloor, TerrainTypeEnum.terrainSlimeFloorBright, TerrainTypeEnum.terrainWaterTar, TerrainTypeEnum.terrainCorruptedTree,
            new List<BuildingLayoutType> { BuildingLayoutType.buildSingleTree, BuildingLayoutType.buildCorruptedForest },
            new List<MonsterLayoutEnum>() { MonsterLayoutEnum.levelBeastsOnly, MonsterLayoutEnum.levelCrimsonDemons, MonsterLayoutEnum.levelMachineDemons,
                MonsterLayoutEnum.levelBeastsAndDemons },
            new List<LevelModifierTypes.LevelModifierEnum>() { LevelModifierTypes.LevelModifierEnum.LevModAngel },
            null,
            (LevelLayout ll, Level level) =>
            {
                for (int i = 0; i < 30; i++)
                {
                    if (UnityEngine.Random.Range(0,100) > 25)
                    {
                        int rx = UnityEngine.Random.Range(1, level.maxX);
                        int ry = UnityEngine.Random.Range(1, level.maxY);

                        if (level.terrain[rx, ry] == TerrainTypeEnum.terrainSlimeFloor)
                            level.terrain[rx, ry] = TerrainTypeEnum.terrainRazorthorns;
                    }
                }

                for (int i = 0; i < 30; i++)
                {
                    if (UnityEngine.Random.Range(0, 100) > 25)
                    {
                        int rx = UnityEngine.Random.Range(1, level.maxX);
                        int ry = UnityEngine.Random.Range(1, level.maxY);

                        if (level.terrain[rx, ry] == TerrainTypeEnum.terrainSlimeFloor)
                            level.terrain[rx, ry] = TerrainTypeEnum.terrainSludgeshrooms;
                    }
                }
            });

        Add(LevelLayoutEnum.levelCityDistrict, "City Residential District", 4,
            TerrainTypeEnum.terrainDirtBorder, TerrainTypeEnum.terrainDirt, TerrainTypeEnum.terrainDirtBright, TerrainTypeEnum.terrainStoneWall,
            TerrainTypeEnum.terrainGrass, TerrainTypeEnum.terrainGrass, TerrainTypeEnum.terrainWater, TerrainTypeEnum.terrainNormalTree,
            new List<BuildingLayoutType> { BuildingLayoutType.buildHouse, BuildingLayoutType.buildWaterPool, BuildingLayoutType.buildNormalForest,
                BuildingLayoutType.buildSingleTree },
            new List<MonsterLayoutEnum>() { MonsterLayoutEnum.levelHumansVsDemons, MonsterLayoutEnum.levelSoldiersVsDemons },
            new List<LevelModifierTypes.LevelModifierEnum>(),
            (LevelLayout ll, Level level, BuildingLayoutType[,] reservedBuildings, MonsterLayoutEnum monsterLayout) =>
            {
                int maxXres = (int)(level.maxX / BuildingLayout.GRID_SIZE);
                int maxYres = (int)(level.maxY / BuildingLayout.GRID_SIZE);


                int startX = maxXres / 2 - 1;
                int startY = maxYres / 2 - 1;
                BuildingLayout bl;
                bl = BuildingLayouts.buildLayouts[BuildingLayoutEnum.buildSoldierPost1];

                List<LevelGenerator.BuildingPlacement> buildingsOnLevel = new List<LevelGenerator.BuildingPlacement>();

                if (monsterLayout == MonsterLayoutEnum.levelSoldiersVsDemons)
                {
                    buildingsOnLevel.Add(new LevelGenerator.BuildingPlacement()
                    {
                        buildType = bl.id,
                        x = startX,
                        y = startY
                    });
                    for (int x = 0; x < bl.gw; x++)
                    {
                        for (int y = 0; y < bl.gh; y++)
                        {
                            reservedBuildings[startX + x, startY + y] = bl.buildType;
                        }
                    }
                }
                return buildingsOnLevel;
            },
            null);

        Add(LevelLayoutEnum.levelAbandonedDistrict, "Abandoned City District", 5,
            TerrainTypeEnum.terrainDirtBorder, TerrainTypeEnum.terrainDirt, TerrainTypeEnum.terrainDirtBright, TerrainTypeEnum.terrainStoneWall,
            TerrainTypeEnum.terrainGrass, TerrainTypeEnum.terrainGrass, TerrainTypeEnum.terrainWater, TerrainTypeEnum.terrainNormalTree,
            new List<BuildingLayoutType> { BuildingLayoutType.buildAbandonedHouse, BuildingLayoutType.buildWaterPool, BuildingLayoutType.buildNormalForest,
                BuildingLayoutType.buildSingleTree },
            new List<MonsterLayoutEnum>() { MonsterLayoutEnum.levelBeastsOnly, MonsterLayoutEnum.levelCrimsonDemons, MonsterLayoutEnum.levelMachineDemons,
                MonsterLayoutEnum.levelBeastsAndDemons, MonsterLayoutEnum.levelSoldiersVsDemons },
            new List<LevelModifierTypes.LevelModifierEnum>(),
            (LevelLayout ll, Level level, BuildingLayoutType[,] reservedBuildings, MonsterLayoutEnum monsterLayout) =>
            {
                int maxXres = (int)(level.maxX / BuildingLayout.GRID_SIZE);
                int maxYres = (int)(level.maxY / BuildingLayout.GRID_SIZE);


                int startX = maxXres / 2 - 1;
                int startY = maxYres / 2 - 1;
                BuildingLayout bl;
                bl = BuildingLayouts.buildLayouts[BuildingLayoutEnum.buildSoldierPost1];

                List<LevelGenerator.BuildingPlacement> buildingsOnLevel = new List<LevelGenerator.BuildingPlacement>();

                if (monsterLayout == MonsterLayoutEnum.levelSoldiersVsDemons)
                {
                    buildingsOnLevel.Add(new LevelGenerator.BuildingPlacement()
                    {
                        buildType = bl.id,
                        x = startX,
                        y = startY
                    });
                    for (int x = 0; x < bl.gw; x++)
                    {
                        for (int y = 0; y < bl.gh; y++)
                        {
                            reservedBuildings[startX + x, startY + y] = bl.buildType;
                        }
                    }
                }
                return buildingsOnLevel;
            },
            null);

        Add(LevelLayoutEnum.levelCorruptedDistrict, "Corrupted City District", 6,
            TerrainTypeEnum.terrainSlimeFloorBorder, TerrainTypeEnum.terrainSlimeFloor, TerrainTypeEnum.terrainSlimeFloorBright, TerrainTypeEnum.terrainStoneWall,
            TerrainTypeEnum.terrainSlimeFloor, TerrainTypeEnum.terrainSlimeFloorBright, TerrainTypeEnum.terrainWater, TerrainTypeEnum.terrainCorruptedTree,
            new List<BuildingLayoutType> { BuildingLayoutType.buildAbandonedHouse, BuildingLayoutType.buildWaterPool, BuildingLayoutType.buildNormalForest,
                BuildingLayoutType.buildCorruptedForest, BuildingLayoutType.buildSingleTree },
            new List<MonsterLayoutEnum>() { MonsterLayoutEnum.levelBeastsOnly, MonsterLayoutEnum.levelCrimsonDemons, MonsterLayoutEnum.levelMachineDemons,
                MonsterLayoutEnum.levelBeastsAndDemons },
            new List<LevelModifierTypes.LevelModifierEnum>(),
            null,
            (LevelLayout ll, Level level) =>
            {
                for (int i = 0; i < 30; i++)
                {
                    if (UnityEngine.Random.Range(0, 100) > 25)
                    {
                        int rx = UnityEngine.Random.Range(1, level.maxX);
                        int ry = UnityEngine.Random.Range(1, level.maxY);

                        if (level.terrain[rx, ry] == TerrainTypeEnum.terrainSlimeFloor)
                            level.terrain[rx, ry] = TerrainTypeEnum.terrainRazorthorns;
                    }
                }

                for (int i = 0; i < 30; i++)
                {
                    if (UnityEngine.Random.Range(0, 100) > 25)
                    {
                        int rx = UnityEngine.Random.Range(1, level.maxX);
                        int ry = UnityEngine.Random.Range(1, level.maxY);

                        if (level.terrain[rx, ry] == TerrainTypeEnum.terrainSlimeFloor)
                            level.terrain[rx, ry] = TerrainTypeEnum.terrainSludgeshrooms;
                    }
                }
            });
    }

    static void Add(LevelLayoutEnum _id, string _name, int _minLvl, TerrainTypeEnum _border, TerrainTypeEnum _floorPrim, TerrainTypeEnum _floorAlt, TerrainTypeEnum _wall, 
        TerrainTypeEnum _grass, TerrainTypeEnum _grassAlt, TerrainTypeEnum _water, TerrainTypeEnum _tree,
        List<BuildingLayoutType> _buildingLayouts, List<MonsterLayoutEnum> _monsterLayouts,
        List<LevelModifierTypes.LevelModifierEnum> _levelModifiers,
        LevelPreProcessFunc _preProcessFunc, LevelPostProcessFunc _postProcessFunc)
    {
        LevelLayout ll = new LevelLayout()
        {
            layoutId = _id,
            name = _name,
            minLvl = _minLvl,
            terrainBorder = _border,
            terrainFloorAlt = _floorAlt,
            terrainFloorPrimary = _floorPrim,
            terrainWall = _wall,
            terrainGrass = _grass,
            terrainGrassAlt = _grassAlt,
            terrainWater = _water,
            terrainTree = _tree,
            buildingLayouts = _buildingLayouts,
            monsterLayouts = _monsterLayouts,
            levelModifiers = _levelModifiers,
            PreProcessFunc = _preProcessFunc,
            PostProcessFunc = _postProcessFunc
        };

        levelLayouts.Add(_id, ll);
    }
}
