using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainType {

    public TerrainTypeEnum id;
    public string name = "Terrain Template";
    public bool blocksMovement = false;
    public bool blocksVision = false;
    public bool blocksProjectiles = false;
    public int catchesFire = 0;
    public Color color = new Color(255, 255, 255);
    public GameObject prefab;

}

public enum TerrainTypeEnum
{
    terrainFogOfWar, terrainFloor, terrainWall, terrainWindow, terrainStoneFloor, terrainStoneFloorBright, terrainStoneFloorBorder, terrainStoneWall,
    terrainWaterTar
};

public class TerrainTypes
{
    public static GameObject terrainFogOfWar;
    public static GameObject terrainFloor;
    public static GameObject terrainWall;
    public static GameObject terrainWindow;
    public static GameObject terrainWaterTar;
    public static Dictionary<TerrainTypeEnum, TerrainType> terrainTypes;

    public static void InitializeTerrainTypes()
    {
        terrainFogOfWar = Resources.Load("Prefabs/Terrains/Fog of war") as GameObject;
        terrainFloor = Resources.Load("Prefabs/Terrains/Floor") as GameObject;
        terrainWall = Resources.Load("Prefabs/Terrains/Wall") as GameObject;
        terrainWindow = Resources.Load("Prefabs/Terrains/Window") as GameObject;
        terrainWaterTar = Resources.Load("Prefabs/Terrains/Tar") as GameObject;

        terrainTypes = new Dictionary<TerrainTypeEnum, TerrainType>();

        Add(TerrainTypeEnum.terrainFloor, "Floor", terrainFloor, false, false, false, new Color(0, 255, 0), 0);
        Add(TerrainTypeEnum.terrainWall, "Wall", terrainWall, true, true, true, new Color(255, 255, 255), 0);
        Add(TerrainTypeEnum.terrainWindow, "Window", terrainWindow, true, false, true, new Color(0, 0, 255), 0);

        Add(TerrainTypeEnum.terrainStoneFloor, "Stone floor", terrainFloor, false, false, false, new Color32(106, 53, 53, 255), 0);
        Add(TerrainTypeEnum.terrainStoneFloorBright, "Stone floor", terrainFloor, false, false, false, new Color32(121, 88, 88, 255), 0);
        Add(TerrainTypeEnum.terrainStoneFloorBorder, "Stone floor", terrainFloor, true, true, true, new Color32(106, 53, 53, 255), 0);
        Add(TerrainTypeEnum.terrainStoneWall, "Stone wall", terrainWall, true, true, true, new Color32(106, 53, 53, 255), 0);
        Add(TerrainTypeEnum.terrainWaterTar, "Tar", terrainWaterTar, false, false, false, new Color32(132, 132, 132, 255), 10);


    }

    private static void Add(TerrainTypeEnum _id, string _name, GameObject _prefab, bool _blocksMovement, bool _blocksVision, bool _blocksProjectiles, Color _color, 
        int _cathesFire)
    {
        TerrainType tt = new TerrainType()
        {
            id = _id,
            name = _name,
            blocksMovement = _blocksMovement,
            blocksVision = _blocksVision,
            blocksProjectiles = _blocksProjectiles,
            prefab = _prefab,
            color = _color,
            catchesFire = _cathesFire
        };
        terrainTypes.Add(_id, tt);
    }
}