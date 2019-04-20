using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelLayoutEnum
{
    levelTest, levelDesolatePlanes, levelTarRiver
}

public delegate List<LevelGenerator.BuildingPlacement> LevelPreProcessFunc(LevelLayout ll, Level level, BuildingLayoutType[,] reservedBuildings);
public delegate List<LevelGenerator.BuildingPlacement> LevelPostProcessFunc(LevelLayout ll, Level level, BuildingLayoutType[,] reservedBuildings);

public class LevelLayout {

    public LevelLayoutEnum layoutId;

    public string name;

    public TerrainTypeEnum terrainBorder;
    public TerrainTypeEnum terrainFloorPrimary;
    public TerrainTypeEnum terrainFloorAlt;
    public TerrainTypeEnum terrainWall;

    public List<BuildingLayoutType> buildingLayouts;

    public LevelPreProcessFunc PreProcessFunc;
    public LevelPostProcessFunc PostProcessFunc;
}

public static class LevelLayouts
{
    public static Dictionary<LevelLayoutEnum, LevelLayout> levelLayouts;

    public static void InitializeLayouts()
    {
        levelLayouts = new Dictionary<LevelLayoutEnum, LevelLayout>();

        Add(LevelLayoutEnum.levelTest, "Test location",
            TerrainTypeEnum.terrainWall, TerrainTypeEnum.terrainStoneFloor, TerrainTypeEnum.terrainStoneFloorBright, TerrainTypeEnum.terrainStoneWall,
            new List<BuildingLayoutType> { BuildingLayoutType.buildEmpty },
            null,
            null);

        Add(LevelLayoutEnum.levelDesolatePlanes, "Desolate plains",
            TerrainTypeEnum.terrainStoneFloorBorder, TerrainTypeEnum.terrainStoneFloor, TerrainTypeEnum.terrainStoneFloorBright, TerrainTypeEnum.terrainStoneWall,
            new List<BuildingLayoutType> { BuildingLayoutType.buildEmpty, BuildingLayoutType.buildShape, BuildingLayoutType.buildTarPool },
            null,
            null);

        Add(LevelLayoutEnum.levelTarRiver, "Tar river",
            TerrainTypeEnum.terrainStoneFloorBorder, TerrainTypeEnum.terrainStoneFloor, TerrainTypeEnum.terrainStoneFloorBright, TerrainTypeEnum.terrainStoneWall,
            new List<BuildingLayoutType> { BuildingLayoutType.buildEmpty, BuildingLayoutType.buildShape },
            (LevelLayout ll, Level level, BuildingLayoutType[,] reservedBuildings) =>
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
    }

    static void Add(LevelLayoutEnum _id, string _name, TerrainTypeEnum _border, TerrainTypeEnum _floorPrim, TerrainTypeEnum _floorAlt, TerrainTypeEnum _wall, 
        List<BuildingLayoutType> _buildingLayouts, LevelPreProcessFunc _preProcessFunc, LevelPostProcessFunc _postProcessFunc)
    {
        LevelLayout ll = new LevelLayout()
        {
            layoutId = _id,
            name = _name,
            terrainBorder = _border,
            terrainFloorAlt = _floorAlt,
            terrainFloorPrimary = _floorPrim,
            terrainWall = _wall,
            buildingLayouts = _buildingLayouts,
            PreProcessFunc = _preProcessFunc,
            PostProcessFunc = _postProcessFunc
        };

        levelLayouts.Add(_id, ll);
    }
}
