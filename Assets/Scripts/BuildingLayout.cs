using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingLayoutEnum
{
    buildEmpty, buildYShape, buildCshape, buildRandom, buildTarPool, buildTarRiverV, buildTarRiverH, buildSingleTree, buildCorruptedForest, buildHouse1,
    buildWaterPool, buildNormalForest, buildSoldierPost1
}

public enum BuildingLayoutType
{
    buildFree, buildNone, buildEmpty, buildShape, buildTarPool, buildSingleTree, buildCorruptedForest, buildHouse, buildWaterPool, buildNormalForest, buildSoldierPost
}

public delegate void BuildingPlaceMobs(Level level, MobTypeEnum mobType, int sx, int sy);

public struct BuildingLayoutResult
{
    public int sx;
    public int sy;

    public struct MobPlacement
    {
        public BuildingPlaceMobs buildingPlaceMob;
        public MobTypeEnum mobType;
    }

    public List<MobPlacement> mobPlacements;
}

public abstract class BuildingLayout {

    public static int GRID_SIZE = 5;

    public BuildingLayoutEnum id;
    public BuildingLayoutType buildType;
    public int gw;
    public int gh;

    public int lw;
    public int lh;

    public abstract BuildingLayoutResult PlaceBuilding(Level level, LevelLayout levelLayout, int x, int y);

    public void TranslateCharsToLevel(Level level, LevelLayout levelLayout, string[] l, int sx, int sy)
    {
        for (int y = 0; y < l.Length; y++)
        {
            for (int x = 0; x < l[y].Length; x++)
            {
                switch (l[y][x])
                {
                    case '.':
                        level.terrain[sx + x, sy + y] = levelLayout.terrainFloorPrimary;
                        if (Random.Range(0, 100) <= 25) level.terrain[sx + x, sy + y] = levelLayout.terrainFloorAlt;
                        break;
                    case ',':
                        level.terrain[sx + x, sy + y] = levelLayout.terrainGrass;
                        if (Random.Range(0, 100) <= 25) level.terrain[sx + x, sy + y] = levelLayout.terrainGrassAlt;
                        break;
                    case '`':
                        level.terrain[sx + x, sy + y] = TerrainTypeEnum.terrainPavement;
                        break;
                    case '#':
                        level.terrain[sx + x, sy + y] = levelLayout.terrainWall;
                        break;
                    case '~':
                        level.terrain[sx + x, sy + y] = levelLayout.terrainWater;
                        break;
                    case 'Y':
                        level.terrain[sx + x, sy + y] = TerrainTypeEnum.terrainTreeWall;
                        break;
                    case 'T':
                        level.terrain[sx + x, sy + y] = TerrainTypeEnum.terrainNormalTree;
                        break;
                    case '-':
                        level.terrain[sx + x, sy + y] = TerrainTypeEnum.terrainWindow;
                        break;
                    case 't':
                        level.terrain[sx + x, sy + y] = TerrainTypeEnum.terrainTable;
                        break;
                    case 'c':
                        level.terrain[sx + x, sy + y] = TerrainTypeEnum.terrainChair;
                        break;
                    case 'b':
                        level.terrain[sx + x, sy + y] = TerrainTypeEnum.terrainBed;
                        break;
                    case 'x':
                        level.terrain[sx + x, sy + y] = TerrainTypeEnum.terrainBarricade;
                        break;
                }
            }
        }
    }
}

public static class BuildingLayouts
{
    public static Dictionary<BuildingLayoutEnum, BuildingLayout> buildLayouts;

    public static void InitializeLayouts()
    {
        buildLayouts = new Dictionary<BuildingLayoutEnum, BuildingLayout>();

        Add(new BuildingLayoutEmpty());
        Add(new BuildingLayoutYShape());
        Add(new BuildingLayoutСShape());
        Add(new BuildingLayoutRandom());
        Add(new BuildingLayoutTarPool());
        Add(new BuildingLayoutTarRiverV());
        Add(new BuildingLayoutTarRiverH());
        Add(new BuildingLayoutSingleTree());
        Add(new BuildingLayoutCorruptedForest());
        Add(new BuildingLayoutHouse());
        Add(new BuildingLayoutNormalForest());
        Add(new BuildingLayoutWaterPool());
        Add(new BuildingLayoutSoldierPost());
    }

    static void Add(BuildingLayout bl)
    {
        buildLayouts.Add(bl.id, bl);
    }
}

public class BuildingLayoutEmpty : BuildingLayout
{
    public BuildingLayoutEmpty()
    {
        gw = 1;
        gh = 1;
        lw = 5;
        lh = 5;
        id = BuildingLayoutEnum.buildEmpty;
        buildType = BuildingLayoutType.buildEmpty;
    }

    public override BuildingLayoutResult PlaceBuilding(Level level, LevelLayout levelLayout, int gx, int gy)
    {
        return new BuildingLayoutResult();
    }
}

public class BuildingLayoutYShape : BuildingLayout
{
    public BuildingLayoutYShape()
    {
        gw = 1;
        gh = 1;
        lw = 5;
        lh = 5;
        id = BuildingLayoutEnum.buildYShape;
        buildType = BuildingLayoutType.buildShape;
    }

    public override BuildingLayoutResult PlaceBuilding(Level level, LevelLayout levelLayout, int x, int y)
    {
        string[] l1 = { ".....",
                        ".#.#.",
                        "..#..",
                        "..#..",
                        "....."};

        string[] l2 = { ".....",
                        "..#..",
                        "..#..",
                        ".#.#.",
                        "....."};

        string[] l;
        int r = Random.Range(0, 2);
        switch (r)
        {
            case 1:
                l = l2;
                break;
            default:
                l = l1;
                break;
        }

        TranslateCharsToLevel(level, levelLayout, l, x, y);

        return new BuildingLayoutResult();
    }
}

public class BuildingLayoutСShape : BuildingLayout
{
    public BuildingLayoutСShape()
    {
        gw = 1;
        gh = 1;
        lw = 5;
        lh = 5;
        id = BuildingLayoutEnum.buildCshape;
        buildType = BuildingLayoutType.buildShape;
    }

    public override BuildingLayoutResult PlaceBuilding(Level level, LevelLayout levelLayout, int x, int y)
    {
        string[] l1 = { ".....",
                        "..##.",
                        ".#...",
                        "..##.",
                        "....."};

        string[] l2 = { ".....",
                        ".##..",
                        "...#.",
                        ".##..",
                        "....."};

        string[] l3 = { ".....",
                        "..#..",
                        ".#.#.",
                        ".#.#.",
                        "....."};

        string[] l4 = { ".....",
                        ".#.#.",
                        ".#.#.",
                        "..#..",
                        "....."};

        string[] l;
        int r = Random.Range(0, 4);
        switch (r)
        {
            case 1:
                l = l2;
                break;
            case 2:
                l = l3;
                break;
            case 3:
                l = l4;
                break;
            default:
                l = l1;
                break;
        }

        TranslateCharsToLevel(level, levelLayout, l, x, y);

        return new BuildingLayoutResult();
    }
}

public class BuildingLayoutRandom : BuildingLayout
{
    public BuildingLayoutRandom()
    {
        gw = 1;
        gh = 1;
        lw = 5;
        lh = 5;
        id = BuildingLayoutEnum.buildRandom;
        buildType = BuildingLayoutType.buildShape;
    }

    public override BuildingLayoutResult PlaceBuilding(Level level, LevelLayout levelLayout, int x, int y)
    {
        for (int i = 0; i < 3; i++) 
        {
            int rx = Random.Range(0, 5);
            int ry = Random.Range(0, 5);

            level.terrain[x + rx, y + ry] = levelLayout.terrainWall;
        }

        return new BuildingLayoutResult();
    }
}

public class BuildingLayoutTarPool : BuildingLayout
{
    public BuildingLayoutTarPool()
    {
        gw = 2;
        gh = 2;
        lw = 8;
        lh = 8;
        id = BuildingLayoutEnum.buildTarPool;
        buildType = BuildingLayoutType.buildTarPool;
    }

    public override BuildingLayoutResult PlaceBuilding(Level level, LevelLayout levelLayout, int x, int y)
    {
        string[] l1 = { "........",
                        "..~~~~..",
                        ".~~~~~~.",
                        "..~~~~~.",
                        ".~~~~~..",
                        "..~~~~..",
                        ".~~~~...",
                        "........" };
                        

        string[] l2 = { "........",
                        "..~~~~..",
                        ".~~~~~~.",
                        ".~~~~~~.",
                        ".~~~~~..",
                        "..~~~~~.",
                        "..~~~~~.",
                        "........" };

        string[] l;
        int r = Random.Range(0, 2);
        switch (r)
        {
            case 1:
                l = l2;
                break;
            default:
                l = l1;
                break;
        }

        TranslateCharsToLevel(level, levelLayout, l, x, y);

        BuildingLayoutResult br = new BuildingLayoutResult()
        {
            mobPlacements = new List<BuildingLayoutResult.MobPlacement>()
        };

        br.mobPlacements.Add(new BuildingLayoutResult.MobPlacement()
        {
            mobType = MobTypeEnum.mobTarDemon,
            buildingPlaceMob = (Level lvl, MobTypeEnum mobType, int sx, int sy) =>
            {
                Mob tarDemon = new Mob(mobType, sx + 5, sy + 5);
                tarDemon.id = BoardManager.instance.FindFreeID(BoardManager.instance.mobs);
                BoardManager.instance.mobs.Add(tarDemon.id, tarDemon);
                lvl.AddMobToLevel(tarDemon, tarDemon.x, tarDemon.y);
            }
        });
        return br;
    }
}

public class BuildingLayoutTarRiverV : BuildingLayout
{
    public BuildingLayoutTarRiverV()
    {
        gw = 2;
        gh = 6;
        lw = 10;
        lh = 30;
        id = BuildingLayoutEnum.buildTarRiverV;
        buildType = BuildingLayoutType.buildNone;
    }

    public override BuildingLayoutResult PlaceBuilding(Level level, LevelLayout levelLayout, int x, int y)
    {
        string[] l =  { ".~~~~~~~~.",
                        ".~~~~~~~~.",
                        ".~~~~~~~~.",
                        "..~~~~~~~.",
                        "..~~~~~~~.",
                        ".~~~~~~~~.",
                        ".~~~~~~~~.",
                        ".~~~~~~~..",
                        ".~~~~~~~..",
                        ".~~~~~~~..",
                        ".~~~~~~~~.",
                        ".~~~~~~~~.",
                        ".~~~~~~~~.",
                        "..~~~~~~~.",
                        "..~~~~~~~.",
                        "..~~~~~~~.",
                        ".~~~~~~~~.",
                        ".~~~~~~~..",
                        ".~~~~~~~..",
                        ".~~~~~~~..",
                        ".~~~~~~~..",
                        ".~~~~~~~~.",
                        ".~~~~~~~~.",
                        "..~~~~~~~.",
                        "..~~~~~~~.",
                        ".~~~~~~~~.",
                        ".~~~~~~~..",
                        ".~~~~~~~..",
                        ".~~~~~~~..",
                        ".~~~~~~~~."};

        TranslateCharsToLevel(level, levelLayout, l, x, y);

        return new BuildingLayoutResult();
    }
}

public class BuildingLayoutTarRiverH : BuildingLayout
{
    public BuildingLayoutTarRiverH()
    {
        gw = 6;
        gh = 2;
        lw = 30;
        lh = 10;
        id = BuildingLayoutEnum.buildTarRiverH;
        buildType = BuildingLayoutType.buildNone;
    }

    public override BuildingLayoutResult PlaceBuilding(Level level, LevelLayout levelLayout, int x, int y)
    {
        string[] l =  { "..............................",
                        "~~..~~~~~~....~~~~~~~....~~~~~",
                        "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~",
                        "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~",
                        "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~",
                        "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~",
                        "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~",
                        "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~",
                        "~~~~...~~~~~~~....~~~~~~~...~~",
                        ".............................." };

        TranslateCharsToLevel(level, levelLayout, l, x, y);

        return new BuildingLayoutResult();
    } 
}

public class BuildingLayoutSingleTree : BuildingLayout
{
    public BuildingLayoutSingleTree()
    {
        gw = 1;
        gh = 1;
        lw = 5;
        lh = 5;
        id = BuildingLayoutEnum.buildSingleTree;
        buildType = BuildingLayoutType.buildSingleTree;
    }

    public override BuildingLayoutResult PlaceBuilding(Level level, LevelLayout levelLayout, int x, int y)
    {
        int rx;
        int ry;
        for (int i = 0; i < 6; i++)
        {
            rx = Random.Range(0, lw);
            ry = Random.Range(0, lh);

            level.terrain[x + rx, y + ry] = TerrainTypeEnum.terrainSlimeFloor;
        }

        string[] l = { ",,,",
                       ",Y,",
                       ",,," };

        rx = Random.Range(0, lw - 3);
        ry = Random.Range(0, lh - 3);

        TranslateCharsToLevel(level, levelLayout, l, x + rx, y + ry);

        return new BuildingLayoutResult();
    }
}

public class BuildingLayoutCorruptedForest : BuildingLayout
{
    public BuildingLayoutCorruptedForest()
    {
        gw = 2;
        gh = 2;
        lw = 8;
        lh = 8;
        id = BuildingLayoutEnum.buildCorruptedForest;
        buildType = BuildingLayoutType.buildCorruptedForest;
    }

    public override BuildingLayoutResult PlaceBuilding(Level level, LevelLayout levelLayout, int x, int y)
    {
        string[] l1 = { "........",
                        ".Y...Y..",
                        "........",
                        "....Y...",
                        "........",
                        "...Y....",
                        ".Y....Y.",
                        "........" };


        string[] l2 = { "........",
                        ".....Y..",
                        ".Y......",
                        "....Y.Y.",
                        "...Y....",
                        ".Y......",
                        "....Y...",
                        "........" };

        string[] l3 = { "........",
                        "...Y....",
                        ".Y...Y..",
                        "........",
                        "..Y...Y.",
                        ".Y......",
                        "...Y.Y..",
                        "........" };

        string[] l;
        int r = Random.Range(0, 3);
        switch (r)
        {
            case 1:
                l = l2;
                break;
            case 2:
                l = l3;
                break;
            default:
                l = l1;
                break;
        }

        TranslateCharsToLevel(level, levelLayout, l, x, y);

        return new BuildingLayoutResult();
    }
}

public class BuildingLayoutHouse : BuildingLayout
{
    public BuildingLayoutHouse()
    {
        gw = 2;
        gh = 2;
        lw = 9;
        lh = 9;
        id = BuildingLayoutEnum.buildHouse1;
        buildType = BuildingLayoutType.buildHouse;
    }

    public override BuildingLayoutResult PlaceBuilding(Level level, LevelLayout levelLayout, int x, int y)
    {
        string[] l1 = { ".........",
                        ".###-###.",
                        ".#```b`#.",
                        ".#`````#.",
                        ".#```tc#.",
                        ".#`````#.",
                        ".#`````#.",
                        ".#-##`##.",
                        "........." };


        string[] l2 = { ".........",
                        ".#######.",
                        ".#``t``#.",
                        ".#``c```.",
                        ".-`````#.",
                        ".#`````#.",
                        ".#`b```-.",
                        ".#######.",
                        "........." };

        string[] l3 = { ".........",
                        ".##`##-#.",
                        ".#`````#.",
                        ".#`````#.",
                        ".#tc```#.",
                        ".#`````#.",
                        ".#```b`#.",
                        ".###-###.",
                        "........." };

        string[] l4 = { ".........",
                        ".#######.",
                        ".-`````#.",
                        ".#````b#.",
                        ".#`````-.",
                        ".```c``#.",
                        ".#``t``#.",
                        ".#######.",
                        "........." };

        string[] l;
        int r = Random.Range(0, 4);
        switch (r)
        {
            case 1:
                l = l2;
                break;
            case 2:
                l = l3;
                break;
            case 3:
                l = l4;
                break;
            default:
                l = l1;
                break;
        }

        TranslateCharsToLevel(level, levelLayout, l, x, y);

        BuildingLayoutResult br = new BuildingLayoutResult()
        {
            mobPlacements = new List<BuildingLayoutResult.MobPlacement>()
        };

        br.mobPlacements.Add(new BuildingLayoutResult.MobPlacement()
        {
            mobType = MobTypeEnum.mobHuman,
            buildingPlaceMob = (Level lvl, MobTypeEnum mobType, int sx, int sy) =>
            {
                Mob human = new Mob(mobType, sx + 2, sy + 3);
                human.id = BoardManager.instance.FindFreeID(BoardManager.instance.mobs);
                BoardManager.instance.mobs.Add(human.id, human);
                lvl.AddMobToLevel(human, human.x, human.y);
            }
        });
        br.mobPlacements.Add(new BuildingLayoutResult.MobPlacement()
        {
            mobType = MobTypeEnum.mobHuman,
            buildingPlaceMob = (Level lvl, MobTypeEnum mobType, int sx, int sy) =>
            {
                Mob human = new Mob(mobType, sx + 5, sy + 5);
                human.id = BoardManager.instance.FindFreeID(BoardManager.instance.mobs);
                BoardManager.instance.mobs.Add(human.id, human);
                lvl.AddMobToLevel(human, human.x, human.y);
            }
        });
        br.mobPlacements.Add(new BuildingLayoutResult.MobPlacement()
        {
            mobType = MobTypeEnum.mobHuman,
            buildingPlaceMob = (Level lvl, MobTypeEnum mobType, int sx, int sy) =>
            {
                Mob human = new Mob(mobType, sx + 6, sy + 6);
                human.id = BoardManager.instance.FindFreeID(BoardManager.instance.mobs);
                BoardManager.instance.mobs.Add(human.id, human);
                lvl.AddMobToLevel(human, human.x, human.y);
            }
        });
        return br;
    }
}

public class BuildingLayoutWaterPool : BuildingLayout
{
    public BuildingLayoutWaterPool()
    {
        gw = 2;
        gh = 2;
        lw = 8;
        lh = 8;
        id = BuildingLayoutEnum.buildWaterPool;
        buildType = BuildingLayoutType.buildWaterPool;
    }

    public override BuildingLayoutResult PlaceBuilding(Level level, LevelLayout levelLayout, int x, int y)
    {
        string[] l1 = { ".,,,,,,.",
                        ".,~~~~,,",
                        ".,,,~~~,",
                        ".,T,~~~,",
                        ".,,,~~,,",
                        ",,~~~,,,",
                        ",~~~~,T,",
                        ",,,,,,,," };


        string[] l2 = { ".,,,,,,,",
                        ",,~~~,T,",
                        ",~~~~,,,",
                        ",~~~~~~,",
                        ",,,~~~,,",
                        ",T,~~~~,",
                        ",,,~~~~,",
                        "..,,,,,," };

        string[] l;
        int r = Random.Range(0, 2);
        switch (r)
        {
            case 1:
                l = l2;
                break;
            default:
                l = l1;
                break;
        }

        TranslateCharsToLevel(level, levelLayout, l, x, y);

        return new BuildingLayoutResult();
    }
}

public class BuildingLayoutNormalForest : BuildingLayout
{
    public BuildingLayoutNormalForest()
    {
        gw = 2;
        gh = 2;
        lw = 8;
        lh = 8;
        id = BuildingLayoutEnum.buildNormalForest;
        buildType = BuildingLayoutType.buildNormalForest;
    }

    public override BuildingLayoutResult PlaceBuilding(Level level, LevelLayout levelLayout, int x, int y)
    {
        string[] l1 = { ",,,.,,,.",
                        ",T,.,T,.",
                        ",,,,,,,.",
                        "...,T,..",
                        "..,,,,..",
                        ",,,T,,,,",
                        ",T,,,,T,",
                        ",,,..,,," };


        string[] l2 = { "....,,,.",
                        ",,,.,T,.",
                        ",T,,,,,,",
                        ",,,,T,T,",
                        ",,,T,,,,",
                        ",T,,,,..",
                        ",,,,T,..",
                        "...,,,.." };

        string[] l3 = { "..,,,...",
                        "..,T,,,.",
                        ".T,,,T,.",
                        ".,,,,,,,",
                        ",,T,.,T,",
                        ",T,,,,,,",
                        ",,,T,T,.",
                        "..,,,,,." };

        string[] l;
        int r = Random.Range(0, 3);
        switch (r)
        {
            case 1:
                l = l2;
                break;
            case 2:
                l = l3;
                break;
            default:
                l = l1;
                break;
        }

        TranslateCharsToLevel(level, levelLayout, l, x, y);

        return new BuildingLayoutResult();
    }
}

public class BuildingLayoutSoldierPost : BuildingLayout
{
    public BuildingLayoutSoldierPost()
    {
        gw = 2;
        gh = 2;
        lw = 7;
        lh = 7;
        id = BuildingLayoutEnum.buildSoldierPost1;
        buildType = BuildingLayoutType.buildSoldierPost;
    }

    public override BuildingLayoutResult PlaceBuilding(Level level, LevelLayout levelLayout, int x, int y)
    {
        string[] l = {  ".......",
                        ".##.##.",
                        ".#...#.",
                        ".......",
                        ".#...#.",
                        ".##.##.",
                        "......." };

        TranslateCharsToLevel(level, levelLayout, l, x, y);

        BuildingLayoutResult br = new BuildingLayoutResult()
        {
            mobPlacements = new List<BuildingLayoutResult.MobPlacement>()
        };

        br.mobPlacements.Add(new BuildingLayoutResult.MobPlacement()
        {
            mobType = MobTypeEnum.mobSoldier,
            buildingPlaceMob = (Level lvl, MobTypeEnum mobType, int sx, int sy) =>
            {
                Mob human = new Mob(mobType, sx + 2, sy + 2);
                human.id = BoardManager.instance.FindFreeID(BoardManager.instance.mobs);
                BoardManager.instance.mobs.Add(human.id, human);
                lvl.AddMobToLevel(human, human.x, human.y);
            }
        });
        br.mobPlacements.Add(new BuildingLayoutResult.MobPlacement()
        {
            mobType = MobTypeEnum.mobSoldier,
            buildingPlaceMob = (Level lvl, MobTypeEnum mobType, int sx, int sy) =>
            {
                Mob human = new Mob(mobType, sx + 4, sy + 2);
                human.id = BoardManager.instance.FindFreeID(BoardManager.instance.mobs);
                BoardManager.instance.mobs.Add(human.id, human);
                lvl.AddMobToLevel(human, human.x, human.y);
            }
        });
        br.mobPlacements.Add(new BuildingLayoutResult.MobPlacement()
        {
            mobType = MobTypeEnum.mobSoldier,
            buildingPlaceMob = (Level lvl, MobTypeEnum mobType, int sx, int sy) =>
            {
                Mob human = new Mob(mobType, sx + 4, sy + 4);
                human.id = BoardManager.instance.FindFreeID(BoardManager.instance.mobs);
                BoardManager.instance.mobs.Add(human.id, human);
                lvl.AddMobToLevel(human, human.x, human.y);
            }
        });
        br.mobPlacements.Add(new BuildingLayoutResult.MobPlacement()
        {
            mobType = MobTypeEnum.mobMachinegunman,
            buildingPlaceMob = (Level lvl, MobTypeEnum mobType, int sx, int sy) =>
            {
                Mob human = new Mob(mobType, sx + 2, sy + 4);
                human.id = BoardManager.instance.FindFreeID(BoardManager.instance.mobs);
                BoardManager.instance.mobs.Add(human.id, human);
                lvl.AddMobToLevel(human, human.x, human.y);
            }
        });
        br.mobPlacements.Add(new BuildingLayoutResult.MobPlacement()
        {
            mobType = MobTypeEnum.mobScout,
            buildingPlaceMob = (Level lvl, MobTypeEnum mobType, int sx, int sy) =>
            {
                Mob human = new Mob(mobType, sx + 3, sy + 3);
                human.id = BoardManager.instance.FindFreeID(BoardManager.instance.mobs);
                BoardManager.instance.mobs.Add(human.id, human);
                lvl.AddMobToLevel(human, human.x, human.y);
            }
        });
        return br;
    }
}