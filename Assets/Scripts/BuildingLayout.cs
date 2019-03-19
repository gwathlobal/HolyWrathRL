using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingLayoutEnum
{
    buildEmpty, buildYShape, buildCshape, buildRandom
}

public struct BuildingLayoutResult
{

}

public abstract class BuildingLayout {

    public static int GRID_SIZE = 5;

    public BuildingLayoutEnum id;
    public int gw;
    public int gh;

    public BuildingLayout(int _gw, int _gh)
    {
        gw = _gw;
        gh = _gh;
    }

    public abstract BuildingLayoutResult PlaceBuilding(Level level, LevelLayout levelLayout, int gx, int gy);

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
                    case '#':
                        level.terrain[sx + x, sy + y] = levelLayout.terrainWall;
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

        Add(new BuildingLayoutEmpty(1, 1));
        Add(new BuildingLayoutYShape(1, 1));
        Add(new BuildingLayoutСShape(1, 1));
        Add(new BuildingLayoutRandom(1, 1));
    }

    static void Add(BuildingLayout bl)
    {
        buildLayouts.Add(bl.id, bl);
    }
}

public class BuildingLayoutEmpty : BuildingLayout
{
    public BuildingLayoutEmpty(int _gw, int _gh) : base(_gw, _gh)
    {
        id = BuildingLayoutEnum.buildEmpty;
    }

    public override BuildingLayoutResult PlaceBuilding(Level level, LevelLayout levelLayout, int gx, int gy)
    {
        return new BuildingLayoutResult();
    }
}

public class BuildingLayoutYShape : BuildingLayout
{
    public BuildingLayoutYShape(int _gw, int _gh) : base(_gw, _gh)
    {
        id = BuildingLayoutEnum.buildYShape;
    }

    public override BuildingLayoutResult PlaceBuilding(Level level, LevelLayout levelLayout, int gx, int gy)
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

        TranslateCharsToLevel(level, levelLayout, l, gx * GRID_SIZE, gy * GRID_SIZE);

        return new BuildingLayoutResult();
    }
}

public class BuildingLayoutСShape : BuildingLayout
{
    public BuildingLayoutСShape(int _gw, int _gh) : base(_gw, _gh)
    {
        id = BuildingLayoutEnum.buildCshape;
    }

    public override BuildingLayoutResult PlaceBuilding(Level level, LevelLayout levelLayout, int gx, int gy)
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

        TranslateCharsToLevel(level, levelLayout, l, gx * GRID_SIZE, gy * GRID_SIZE);

        return new BuildingLayoutResult();
    }
}

public class BuildingLayoutRandom : BuildingLayout
{
    public BuildingLayoutRandom(int _gw, int _gh) : base(_gw, _gh)
    {
        id = BuildingLayoutEnum.buildRandom;
    }

    public override BuildingLayoutResult PlaceBuilding(Level level, LevelLayout levelLayout, int gx, int gy)
    {
        for (int i = 0; i < 3; i++) 
        {
            int rx = Random.Range(0, 5);
            int ry = Random.Range(0, 5);

            level.terrain[gx * GRID_SIZE + rx, gy * GRID_SIZE + ry] = levelLayout.terrainWall;
        }

        return new BuildingLayoutResult();
    }
}
