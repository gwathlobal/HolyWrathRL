using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TerrainOnStep(Level level, Mob mob);

public class TerrainType {

    public TerrainTypeEnum id;
    public string name = "Terrain Template";
    public bool blocksMovement = false;
    public bool blocksVision = false;
    public bool blocksProjectiles = false;
    public int catchesFire = 0;
    public TerrainTypeEnum burnsToTerrain;
    public Color color = new Color(255, 255, 255);
    public GameObject prefab;

    public TerrainOnStep TerrainOnStep;
}

public enum TerrainTypeEnum
{
    terrainFloor, terrainWall, terrainWindow,
    terrainStoneFloorBorder, terrainWaterTarBorder, terrainSlimeFloorBorder,
    terrainStoneFloor, terrainStoneFloorBright, terrainAshes, terrainSlimeFloor, terrainSlimeFloorBright,
    terrainStoneWall, terrainTreeWall,
    terrainWaterTar, terrainRazorthorns, terrainSludgeshrooms
};

public class TerrainTypes
{
    public static GameObject terrainFloor;
    public static GameObject terrainWall;
    public static GameObject terrainWindow;
    public static GameObject terrainWaterTar;
    public static GameObject terrainCorruptedTree;
    public static GameObject terrainBush;
    public static GameObject terrainShroom;
    public static Dictionary<TerrainTypeEnum, TerrainType> terrainTypes;

    public static void InitializeTerrainTypes()
    {
        terrainFloor = Resources.Load("Prefabs/Terrains/Floor") as GameObject;
        terrainWall = Resources.Load("Prefabs/Terrains/Wall") as GameObject;
        terrainWindow = Resources.Load("Prefabs/Terrains/Window") as GameObject;
        terrainWaterTar = Resources.Load("Prefabs/Terrains/Tar") as GameObject;
        terrainCorruptedTree = Resources.Load("Prefabs/Terrains/Corrupted Tree") as GameObject;
        terrainBush = Resources.Load("Prefabs/Terrains/Bush") as GameObject;
        terrainShroom = Resources.Load("Prefabs/Terrains/Shroom") as GameObject;

        terrainTypes = new Dictionary<TerrainTypeEnum, TerrainType>();

        Add(TerrainTypeEnum.terrainFloor, "Floor", terrainFloor, false, false, false, new Color(0, 255, 0), 0, TerrainTypeEnum.terrainFloor, 
            null);
        Add(TerrainTypeEnum.terrainWall, "Wall", terrainWall, true, true, true, new Color(255, 255, 255), 0, TerrainTypeEnum.terrainWall, 
            null);
        Add(TerrainTypeEnum.terrainWindow, "Window", terrainWindow, true, false, true, new Color(0, 0, 255), 0, TerrainTypeEnum.terrainWindow, 
            null);

        // floors

        Add(TerrainTypeEnum.terrainStoneFloor, "Stone floor", terrainFloor, false, false, false, new Color32(106, 53, 53, 255), 0, TerrainTypeEnum.terrainStoneFloor, 
            null);

        Add(TerrainTypeEnum.terrainStoneFloorBright, "Stone floor", terrainFloor, false, false, false, new Color32(121, 88, 88, 255), 0, TerrainTypeEnum.terrainStoneFloorBright,
            null);

        Add(TerrainTypeEnum.terrainSlimeFloor, "Slime floor", terrainFloor, false, false, false, new Color32(110, 0, 255, 255), 0, TerrainTypeEnum.terrainSlimeFloor,
            null);

        Add(TerrainTypeEnum.terrainSlimeFloorBright, "Slime floor", terrainFloor, false, false, false, new Color32(255, 0, 255, 255), 0, TerrainTypeEnum.terrainSlimeFloorBright,
            null);

        // borders

        Add(TerrainTypeEnum.terrainStoneFloorBorder, "Stone floor", terrainFloor, true, true, true, new Color32(106, 53, 53, 255), 0, TerrainTypeEnum.terrainStoneFloorBorder, 
            null);

        Add(TerrainTypeEnum.terrainWaterTarBorder, "Tar", terrainWaterTar, true, true, true, new Color32(132, 132, 132, 255), 0, TerrainTypeEnum.terrainWaterTarBorder,
            null);

        Add(TerrainTypeEnum.terrainSlimeFloorBorder, "Slime floor", terrainFloor, true, true, true, new Color32(110, 0, 255, 255), 0, TerrainTypeEnum.terrainSlimeFloorBorder,
            null);

        // walls

        Add(TerrainTypeEnum.terrainStoneWall, "Stone wall", terrainWall, true, true, true, new Color32(106, 53, 53, 255), 0, TerrainTypeEnum.terrainStoneWall, 
            null);

        Add(TerrainTypeEnum.terrainTreeWall, "Twintube tree", terrainCorruptedTree, true, true, true, new Color32(110, 0, 255, 255), 0, TerrainTypeEnum.terrainTreeWall,
            null);

        // other

        Add(TerrainTypeEnum.terrainWaterTar, "Tar", terrainWaterTar, false, false, false, new Color32(132, 132, 132, 255), 10, TerrainTypeEnum.terrainAshes,
            (Level level, Mob mob) =>
            {
                mob.AddEffect(EffectTypeEnum.effectCoveredInTar, mob, 5);
            });

        Add(TerrainTypeEnum.terrainAshes, "Ashes", terrainWaterTar, false, false, false, new Color32(70, 70, 70, 255), 0, TerrainTypeEnum.terrainAshes, 
            null);

        Add(TerrainTypeEnum.terrainRazorthorns, "Razorthorns", terrainBush, false, false, false, new Color32(180, 54, 0, 255), 5, TerrainTypeEnum.terrainAshes,
            (Level level, Mob actor) =>
            {
                string str = System.String.Format("{0} steps into razorthorns. ", actor.name);
                BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

                int dmg = 0;
                dmg += Mob.InflictDamage(null, actor, 5, DmgTypeEnum.Physical, null);
                if (BoardManager.instance.level.visible[actor.x, actor.y])
                {
                    Vector3 pos = new Vector3(actor.x, actor.y, 0);
                    UIManager.instance.CreateFloatingText(dmg + " <i>DMG</i>", pos);
                }
                BoardManager.instance.CreateBlooddrop(actor.x, actor.y);
                if (actor.CheckDead())
                {
                    actor.MakeDead(null, true, true, false);
                }
            });

        Add(TerrainTypeEnum.terrainSludgeshrooms, "Sludgeshrooms", terrainShroom, false, false, false, new Color32(215, 241, 0, 255), 5, TerrainTypeEnum.terrainAshes,
            (Level level, Mob actor) =>
            {
                if (UnityEngine.Random.Range(0, 100) <= 20)
                {
                    string str = System.String.Format("{0} steps into sludgshrooms and they release toxic spores. ", actor.name);
                    BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

                    level.CheckSurroundings(actor.x, actor.y, true,
                        (int x, int y) =>
                        {
                            Feature cloud = new Feature(FeatureTypeEnum.featAcidCloud, x, y);
                            cloud.counter = 5;
                            BoardManager.instance.level.AddFeatureToLevel(cloud, cloud.x, cloud.y);
                        });
                }
            });
    }

    private static void Add(TerrainTypeEnum _id, string _name, GameObject _prefab, bool _blocksMovement, bool _blocksVision, bool _blocksProjectiles, Color _color, 
        int _cathesFire, TerrainTypeEnum _burnsTo, TerrainOnStep _terrainOnStep)
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
            catchesFire = _cathesFire,
            burnsToTerrain = _burnsTo,
            TerrainOnStep = _terrainOnStep
        };
        terrainTypes.Add(_id, tt);
    }
}