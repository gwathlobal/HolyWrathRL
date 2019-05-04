using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void make_connect(int x, int y);

public delegate bool check_connect(int x, int y, int px, int py);

public delegate void surround_func(int x, int y);

public class Level {

    public TerrainTypeEnum[,] terrain;
    public Mob[,] mobs;
    public List<Item>[,] items;
    public List<Feature>[,] features;
    public bool[,] visible;
    public int[,] connected;

    public int maxX = 30;
    public int maxY = 30;

    public List<Mob> mobList;
    public List<Item> itemList;
    public List<Feature> featureList;

    public ObjectiveLayoutEnum objectiveType;

    public const int roomNotConnected = 0;

    public Dictionary<MobTypeEnum, int> mobsToSpawn;

    private static List<Vector2Int> offsetList = new List<Vector2Int> { new Vector2Int(-1, -1),
                                                                        new Vector2Int(-1, 0),
                                                                        new Vector2Int(-1, 1),
                                                                        new Vector2Int(0, -1),
                                                                        new Vector2Int(0, 0),
                                                                        new Vector2Int(0, 1),
                                                                        new Vector2Int(1, -1),
                                                                        new Vector2Int(1, 0),
                                                                        new Vector2Int(1, 1)
                                                                      };

    public Level(LevelLayoutEnum levelLayoutType, MonsterLayoutEnum monsterLayoutType, ObjectiveLayoutEnum objectiveLayoutType)
    {
        terrain = new TerrainTypeEnum[maxX, maxY];
        items = new List<Item>[maxX, maxY];
        features = new List<Feature>[maxX, maxY];
        visible = new bool[maxX, maxY];
        mobs = new Mob[maxX, maxY];
        connected = new int[maxX, maxY];

        mobList = new List<Mob>();
        itemList = new List<Item>();
        featureList = new List<Feature>();

        // initialize items array
        for (int y = 0; y < maxY; y++)
        {
            for (int x = 0; x < maxX; x++)
            {
                items[x, y] = new List<Item>();
                features[x, y] = new List<Feature>();
            }
        }

        GenerateLevel(levelLayoutType, monsterLayoutType);

        CalculateConnectivity();

        objectiveType = objectiveLayoutType;
    }

    public bool AddMobToLevel(Mob mob, int x, int y)
    {
        if (mobs[x, y] != null) return false;
        if (mobList.Contains(mob)) return false;
        mob.x = x;
        mob.y = y;
        mobList.Add(mob);
        mobs[x, y] = mob;
        mob.go.SetActive(true);
        //mob.go.GetComponent<SpriteRenderer>().color = MobTypes.mobTypes[mob.idType].prefab.GetComponent<SpriteRenderer>().color;
        mob.go.GetComponent<Rigidbody2D>().MovePosition(new Vector2(x, y));
        return true;
    }

    public bool RemoveMobFromLevel(Mob mob)
    {
        if (mob == null) return false;
        if (!mobList.Contains(mob)) return false;

        mobs[mob.x, mob.y] = null;
        mobList.Remove(mob);
        //mob.go.GetComponent<SpriteRenderer>().color = new Color32(0, 0, 0, 0);
        BoardAnimationController.instance.AddAnimationProcedure(new AnimationProcedure(mob.go, () => {
            //Debug.Log("Removing mob " + mob.name + " from level");
            mob.go.SetActive(false);
            BoardAnimationController.instance.RemoveProcessedAnimation();
        }));
        
        return true;
    }

    public bool AddItemToLevel(Item item, int x, int y)
    {
        if (itemList.Contains(item)) return false;
        item.x = x;
        item.y = y;
        itemList.Add(item);
        items[x, y].Add(item);
        item.go.SetActive(true);
        item.go.GetComponent<Transform>().position = new Vector2(x, y);
        return true;
    }

    public bool RemoveItemFromLevel(Item item)
    {
        if (item == null) return false;
        if (!itemList.Contains(item)) return false;

        items[item.x, item.y].Remove(item);
        itemList.Remove(item);
        item.go.SetActive(false);
        return true;
    }

    public bool AddFeatureToLevel(Feature feature, int x, int y)
    {
        if (featureList.Contains(feature)) return false;
        feature.x = x;
        feature.y = y;

        Feature mergeResult = null;
        if (FeatureTypes.featureTypes[feature.idType].FeatCheckMerge != null)
            mergeResult = FeatureTypes.featureTypes[feature.idType].FeatCheckMerge(BoardManager.instance.level, feature);

        if (mergeResult != null)
        {
            if (FeatureTypes.featureTypes[feature.idType].FeatMergeFunc != null)
                FeatureTypes.featureTypes[feature.idType].FeatMergeFunc(BoardManager.instance.level, feature, mergeResult);
        }
        else
        {
            featureList.Add(feature);
            features[x, y].Add(feature);
            if (feature.go != null)
            {
                feature.go.SetActive(true);
                feature.go.GetComponent<Transform>().position = new Vector2(x, y);
            }
        }
        return true;
    }

    public bool RemoveFeatureFromLevel(Feature feature)
    {
        if (feature == null) return false;
        if (!featureList.Contains(feature)) return false;

        features[feature.x, feature.y].Remove(feature);
        featureList.Remove(feature);
        if (feature.go != null)
        {
            feature.go.SetActive(false);
        }
        return true;
    }

    private void FloodFill(int sx, int sy, check_connect CheckConnect, make_connect MakeConnect)
    {
        Stack<Vector2Int> openCells = new Stack<Vector2Int>();
        openCells.Push(new Vector2Int(sx, sy));
        Vector2Int curCell;

        int x, y;

        do
        {
            curCell = openCells.Pop();
            MakeConnect(curCell.x, curCell.y);

            foreach (Vector2Int offset in offsetList)
            {
                x = curCell.x + offset.x;
                y = curCell.y + offset.y;
                if (x >= 0 && y >= 0 && x < maxX && y < maxY &&
                    !(offset.x == 0 && offset.y == 0) &&
                    CheckConnect(x,y,curCell.x,curCell.y))
                {
                    openCells.Push(new Vector2Int(x, y));
                }
            }

        } while (openCells.Count > 0);
    }

    public void CalculateConnectivity()
    {
        int roomId = 1;
        check_connect checkFunc = (int x, int y, int px, int p) =>
        {

            bool result = true;

            if (x >= 0 && y >= 0 && x < maxX && y < maxY &&
                TerrainTypes.terrainTypes[terrain[x,y]].blocksMovement)
            {
                result = false;
            }

            if (connected[x,y] == roomNotConnected && result)
                return true;
            else
                return false;
        };

        make_connect makeFunc = (int x, int y) =>
        {
            connected[x, y] = roomId;
        };

        for (int y = 0; y < maxY; y++)
        {
            for (int x = 0; x < maxX; x++)
            {
                if (checkFunc(x, y, x, y))
                {
                    FloodFill(x, y, checkFunc, makeFunc);
                    roomId++;
                }
            }
        }
    }

    public bool AreCellsConnected(int sx, int sy, int tx, int ty)
    {
        if (connected[sx, sy] != roomNotConnected && connected[tx, ty] != roomNotConnected && connected[sx, sy] == connected[tx, ty])
            return true;
        else
            return false;
    }
        
    public void CheckSurroundings(int sx, int sy, bool includeCenter, surround_func SurroundFunc)
    {
        int x;
        int y;
        bool doFunc;

        foreach (Vector2Int offset in offsetList)
        {
            x = sx + offset.x;
            y = sy + offset.y;
            doFunc = true;
            if (offset.x == 0 && offset.y == 0 && !includeCenter) doFunc = false;
            if (x >= 0 && y >= 0 && x < maxX && y < maxY && doFunc)
            {
                SurroundFunc(x, y);
            }
        }
    }

    public static double GetDistance(int sx, int sy, int tx, int ty)
    {
        return System.Math.Sqrt(((double)sx - (double)tx) * ((double)sx - (double)tx) + ((double)sy - (double)ty) * ((double)sy - (double)ty));
    }

    public static int GetSimpleDistance(int sx, int sy, int tx, int ty)
    {
        if (System.Math.Abs(sx - tx) > System.Math.Abs(sy - ty)) return System.Math.Abs(sx - tx);
        else return System.Math.Abs(sy - ty);
    }

    public void GenerateLevel(LevelLayoutEnum levelLayoutType, MonsterLayoutEnum monsterLayoutType)
    {
        LevelLayout levelLayout = LevelLayouts.levelLayouts[levelLayoutType];
        MonsterLayout monsterLayout = MonsterLayouts.monsterLayouts[monsterLayoutType];
        
        for (int y = 0; y < maxY; y++)
        {
            for (int x = 0; x < maxX; x++)
            {
                terrain[x, y] = levelLayout.terrainFloorPrimary;
                if (Random.Range(0, 100) <= 25) terrain[x, y] = levelLayout.terrainFloorAlt;
            }
        }

        LevelGeneratorResult levelGeneratorResult = LevelGenerator.GenerateLevel(this, levelLayout);

        monsterLayout.PlaceMobs(this, levelGeneratorResult);

        // place buildings from pre-processed function
        if (levelLayout.PostProcessFunc != null)
            levelLayout.PostProcessFunc(levelLayout, this);
    }

    public bool FindFreeSpotInside(out Vector2Int loc)
    {
        int i = 0;
        int rx, ry;
        do
        {
            i++;
            rx = Random.Range(2, maxX - 3);
            ry = Random.Range(2, maxY - 3);
            if (i >= 200) break;
        } while (TerrainTypes.terrainTypes[terrain[rx, ry]].blocksMovement == true || mobs[rx, ry] != null);

        loc = new Vector2Int(rx, ry);
        if (i < 200) return true;
        else return false;
    }

    public bool FindFreeSpotAtBorder(out Vector2Int loc)
    {
        int i = 0;
        int rx, ry;
        int side;
        do
        {
            i++;
            side = Random.Range(0, 4);
            switch (side)
            {
                case 0:
                    rx = 1;
                    ry = Random.Range(1, maxY - 2);
                    break;
                case 1:
                    rx = Random.Range(1, maxX - 2);
                    ry = 1;
                    break;
                case 2:
                    rx = maxX - 2;
                    ry = Random.Range(1, maxY - 2);
                    break;
                default:
                    rx = Random.Range(1, maxX - 2);
                    ry = maxY - 2;
                    break;
            }
            if (i >= 200) break;
        } while (IsTerrainImpassable(rx, ry) || mobs[rx, ry] != null);

        loc = new Vector2Int(rx, ry);
        if (i < 200) return true;
        else return false;
    }

    public bool IsTerrainImpassable(int x, int y)
    {
        return TerrainTypes.terrainTypes[terrain[x, y]].blocksMovement;
    }
}
