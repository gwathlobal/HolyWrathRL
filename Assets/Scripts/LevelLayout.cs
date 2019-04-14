using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelLayoutEnum
{
    levelTest, levelNormal
}


public class LevelLayout {

    public LevelLayoutEnum layoutId;

    public string name;

    public TerrainTypeEnum terrainBorder;
    public TerrainTypeEnum terrainFloorPrimary;
    public TerrainTypeEnum terrainFloorAlt;
    public TerrainTypeEnum terrainWall;

    public List<BuildingLayoutType> buildingLayouts;
}

public static class LevelLayouts
{
    public static Dictionary<LevelLayoutEnum, LevelLayout> levelLayouts;

    public static void InitializeLayouts()
    {
        levelLayouts = new Dictionary<LevelLayoutEnum, LevelLayout>();

        Add(LevelLayoutEnum.levelTest, "Test location",
            TerrainTypeEnum.terrainWall, TerrainTypeEnum.terrainStoneFloor, TerrainTypeEnum.terrainStoneFloorBright, TerrainTypeEnum.terrainStoneWall,
            new List<BuildingLayoutType> { BuildingLayoutType.buildEmpty });

        Add(LevelLayoutEnum.levelNormal, "Ordinary location",
            TerrainTypeEnum.terrainStoneFloorBorder, TerrainTypeEnum.terrainStoneFloor, TerrainTypeEnum.terrainStoneFloorBright, TerrainTypeEnum.terrainStoneWall,
            new List<BuildingLayoutType> { BuildingLayoutType.buildEmpty, BuildingLayoutType.buildShape, BuildingLayoutType.buildTarPool });
    }

    static void Add(LevelLayoutEnum _id, string _name, TerrainTypeEnum _border, TerrainTypeEnum _floorPrim, TerrainTypeEnum _floorAlt, TerrainTypeEnum _wall, 
        List<BuildingLayoutType> _buildingLayouts)
    {
        LevelLayout ll = new LevelLayout()
        {
            layoutId = _id,
            name = _name,
            terrainBorder = _border,
            terrainFloorAlt = _floorAlt,
            terrainFloorPrimary = _floorPrim,
            terrainWall = _wall,
            buildingLayouts = _buildingLayouts
        };

        levelLayouts.Add(_id, ll);
    }
}
