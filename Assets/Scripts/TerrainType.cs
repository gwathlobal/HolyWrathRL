using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TerrainOnStep(Level level, Mob mob);

public class TerrainType {

    public TerrainTypeEnum id;
    public string name = "Terrain Template";
    public string descr = "";
    public bool blocksMovement = false;
    public bool blocksVision = false;
    public bool blocksProjectiles = false;
    public int catchesFire = 0;
    public TerrainTypeEnum burnsToTerrain;
    public Color color = new Color(255, 255, 255);
    public GameObject prefab;

    public TerrainOnStep TerrainOnStep;

    public string Description()
    {
        string str = "";

        str += System.String.Format("{0}\n{1}\n", name, (descr != "") ? descr + "\n": "");
        str += System.String.Format("   Blocks movement: {0}\n", (blocksMovement) ? "true" : "false");
        str += System.String.Format("   Blocks vision: {0}\n", (blocksVision) ? "true" : "false");
        str += System.String.Format("   Blocks projectiles: {0}\n", (blocksProjectiles) ? "true" : "false");
        str += System.String.Format("   Can burn: {0}\n", (catchesFire > 0) ? "true" : "false");

        return str;
    }
}

public enum TerrainTypeEnum
{
    terrainFloor, terrainWall, terrainWindow,
    terrainStoneFloorBorder, terrainWaterTarBorder, terrainSlimeFloorBorder,
    terrainStoneFloor, terrainStoneFloorBright, terrainAshes, terrainSlimeFloor, terrainSlimeFloorBright,
    terrainStoneWall, terrainCorruptedTree,
    terrainWaterTar, terrainRazorthorns, terrainSludgeshrooms,
    terrainGrass, terrainDirt, terrainDirtBright, terrainDirtBorder, terrainPavement, terrainBed, terrainChair, terrainTable, terrainWater, terrainNormalTree,
    terrainBarricade
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
    public static GameObject terrainPavement;
    public static GameObject terrainChair;
    public static GameObject terrainBed;
    public static GameObject terrainTable;
    public static GameObject terrainNormalTree;
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
        terrainPavement = Resources.Load("Prefabs/Terrains/Pavement") as GameObject;
        terrainChair = Resources.Load("Prefabs/Terrains/Chair") as GameObject;
        terrainBed = Resources.Load("Prefabs/Terrains/Bed") as GameObject;
        terrainTable = Resources.Load("Prefabs/Terrains/Table") as GameObject;
        terrainNormalTree = Resources.Load("Prefabs/Terrains/Normal Tree") as GameObject;

        terrainTypes = new Dictionary<TerrainTypeEnum, TerrainType>();

        Add(TerrainTypeEnum.terrainFloor, "Floor", "", terrainFloor, false, false, false, new Color(0, 255, 0), 0, TerrainTypeEnum.terrainFloor, 
            null);
        Add(TerrainTypeEnum.terrainWall, "Wall", "", terrainWall, true, true, true, new Color(255, 255, 255), 0, TerrainTypeEnum.terrainWall, 
            null);
        

        // floors

        Add(TerrainTypeEnum.terrainStoneFloor, "Stone floor", "", terrainFloor, false, false, false, new Color32(106, 53, 53, 255), 0, TerrainTypeEnum.terrainStoneFloor, 
            null);

        Add(TerrainTypeEnum.terrainStoneFloorBright, "Stone floor", "", terrainFloor, false, false, false, new Color32(121, 88, 88, 255), 0, TerrainTypeEnum.terrainStoneFloorBright,
            null);

        Add(TerrainTypeEnum.terrainSlimeFloor, "Slime floor", "", terrainFloor, false, false, false, new Color32(110, 0, 255, 255), 0, TerrainTypeEnum.terrainSlimeFloor,
            null);

        Add(TerrainTypeEnum.terrainSlimeFloorBright, "Slime floor", "", terrainFloor, false, false, false, new Color32(255, 0, 255, 255), 0, TerrainTypeEnum.terrainSlimeFloorBright,
            null);

        Add(TerrainTypeEnum.terrainDirt, "Dirt", "", terrainFloor, false, false, false, new Color32(205, 103, 63, 255), 0, TerrainTypeEnum.terrainDirt,
            null);

        Add(TerrainTypeEnum.terrainDirtBright, "Dirt", "", terrainFloor, false, false, false, new Color32(139, 69, 19, 255), 0, TerrainTypeEnum.terrainDirtBright,
            null);

        Add(TerrainTypeEnum.terrainGrass, "Grass", "", terrainFloor, false, false, false, new Color32(0, 100, 0, 255), 3, TerrainTypeEnum.terrainAshes,
            null);

        Add(TerrainTypeEnum.terrainPavement, "Floor", "", terrainPavement, false, false, false, new Color32(200, 200, 200, 255), 0, TerrainTypeEnum.terrainPavement,
            null);

        // borders

        Add(TerrainTypeEnum.terrainStoneFloorBorder, "Stone floor", "", terrainFloor, true, true, true, new Color32(106, 53, 53, 255), 0, TerrainTypeEnum.terrainStoneFloorBorder, 
            null);

        Add(TerrainTypeEnum.terrainWaterTarBorder, "Tar", "", terrainWaterTar, true, true, true, new Color32(132, 132, 132, 255), 0, TerrainTypeEnum.terrainWaterTarBorder,
            null);

        Add(TerrainTypeEnum.terrainSlimeFloorBorder, "Slime floor", "", terrainFloor, true, true, true, new Color32(110, 0, 255, 255), 0, TerrainTypeEnum.terrainSlimeFloorBorder,
            null);

        Add(TerrainTypeEnum.terrainDirtBorder, "Dirt", "", terrainFloor, true, true, true, new Color32(205, 103, 63, 255), 0, TerrainTypeEnum.terrainDirtBorder,
            null);

        // walls

        Add(TerrainTypeEnum.terrainStoneWall, "Stone wall", "", terrainWall, true, true, true, new Color32(106, 53, 53, 255), 0, TerrainTypeEnum.terrainStoneWall, 
            null);

        Add(TerrainTypeEnum.terrainCorruptedTree, "Twintube tree", "", terrainCorruptedTree, true, true, true, new Color32(110, 0, 255, 255), 0, TerrainTypeEnum.terrainCorruptedTree,
            null);

        Add(TerrainTypeEnum.terrainNormalTree, "Birch tree", "", terrainNormalTree, true, true, true, new Color32(79, 214, 0, 255), 0, TerrainTypeEnum.terrainNormalTree,
            null);

        // other

        Add(TerrainTypeEnum.terrainWaterTar, "Tar", "Any creatures that walks here gets covered in tar.", terrainWaterTar, false, false, false, new Color32(132, 132, 132, 255), 10, TerrainTypeEnum.terrainAshes,
            (Level level, Mob mob) =>
            {
                mob.AddEffect(EffectTypeEnum.effectCoveredInTar, mob, 5);
            });

        Add(TerrainTypeEnum.terrainWater, "Water", "", terrainWaterTar, true, false, false, new Color32(0, 0, 255, 255), 0, TerrainTypeEnum.terrainWater,
            null);

        Add(TerrainTypeEnum.terrainWindow, "Window", "", terrainWindow, true, false, true, new Color32(0, 0, 255, 255), 0, TerrainTypeEnum.terrainWindow,
            null);

        Add(TerrainTypeEnum.terrainBed, "Bed", "", terrainBed, true, false, false, new Color32(139, 69, 19, 255), 0, TerrainTypeEnum.terrainBed,
            null);

        Add(TerrainTypeEnum.terrainChair, "Chair", "", terrainChair, true, false, false, new Color32(139, 69, 19, 255), 0, TerrainTypeEnum.terrainChair,
            null);

        Add(TerrainTypeEnum.terrainTable, "Table", "", terrainTable, true, false, false, new Color32(139, 69, 19, 255), 0, TerrainTypeEnum.terrainTable,
            null);

        Add(TerrainTypeEnum.terrainAshes, "Ashes", "", terrainWaterTar, false, false, false, new Color32(70, 70, 70, 255), 0, TerrainTypeEnum.terrainAshes, 
            null);

        Add(TerrainTypeEnum.terrainBarricade, "Barricade", "", terrainBush, true, false, false, new Color32(139, 69, 19, 255), 0, TerrainTypeEnum.terrainBarricade,
            null);

        Add(TerrainTypeEnum.terrainRazorthorns, "Razorthorns", "Any creature that steps into razorthorns takes 5 physical damage.", terrainBush, false, false, false, new Color32(180, 54, 0, 255), 5, TerrainTypeEnum.terrainAshes,
            (Level level, Mob actor) =>
            {
                string str = System.String.Format("{0} steps into razorthorns. ", actor.name);
                BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

                int dmg = 0;
                dmg += Mob.InflictDamage(null, actor,
                    new Dictionary<DmgTypeEnum, int>()
                    {
                        { DmgTypeEnum.Physical, 5 }
                    }, 
                    null);
                if (BoardManager.instance.level.visible[actor.x, actor.y])
                {
                    Vector3 pos = new Vector3(actor.x, actor.y, 0);
                    UIManager.instance.CreateFloatingText(dmg + " <i>DMG</i>", pos);
                }
                BoardManager.instance.CreateBlooddrop(actor.x, actor.y);
                if (actor.CheckDead())
                {
                    actor.MakeDead(null, true, true, false, "Killed by razorthorns.");
                }
            });

        Add(TerrainTypeEnum.terrainSludgeshrooms, "Sludgeshrooms", "Any creatures that steps into sludgeshrooms has 20% chance to release a cloud of toxic spores that deal 10 acid damage.", terrainShroom, false, false, false, new Color32(215, 241, 0, 255), 5, TerrainTypeEnum.terrainAshes,
            (Level level, Mob actor) =>
            {
                if (UnityEngine.Random.Range(0, 100) <= 20)
                {
                    string str = System.String.Format("{0} steps into sludgshrooms and they release toxic spores. ", actor.name);
                    BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

                    level.CheckSurroundings(actor.x, actor.y, true,
                        (int x, int y) =>
                        {
                            if (!TerrainTypes.terrainTypes[BoardManager.instance.level.terrain[x, y]].blocksMovement)
                            {
                                Feature cloud = new Feature(FeatureTypeEnum.featAcidCloud, x, y);
                                cloud.counter = 5;
                                BoardManager.instance.level.AddFeatureToLevel(cloud, cloud.x, cloud.y);
                            }
                        });
                }
            });
    }

    private static void Add(TerrainTypeEnum _id, string _name, string _descr, GameObject _prefab, bool _blocksMovement, bool _blocksVision, bool _blocksProjectiles, Color _color, 
        int _cathesFire, TerrainTypeEnum _burnsTo, TerrainOnStep _terrainOnStep)
    {
        TerrainType tt = new TerrainType()
        {
            id = _id,
            name = _name,
            descr = _descr,
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