using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventLevelShrink : GameEvent
{
    public int turnsLeft = 0;
    public int initTurns = 0;
    public Level level;

    private int steps;
    private int offset;

    public EventLevelShrink()
    {
        idType = GameEventTypes.GameEventEnum.eventShrinkLevel;
        name = "Shrinking dimension";
    }

    public override void Activate()
    {
        turnsLeft--;

        if (turnsLeft % (initTurns / steps) != 0) return;

        string str = System.String.Format("The pocket dimension is shrinking! ");
        BoardManager.instance.msgLog.PlayerVisibleMsg(BoardManager.instance.player.x, BoardManager.instance.player.y, str);
        BoardManager.instance.msgLog.FinalizeMsg();

        Level level = BoardManager.instance.level;
        bool[,] tempLevel = new bool[level.maxX, level.maxY];


        // prepare a temp level, true means "put void", false means "remain unchanged"
        for (int x = 0; x < level.maxX; x++)
        {
            for (int y = 0; y < level.maxY; y++)
            {
                tempLevel[x, y] = false;
            }
        }

        // draw a line in the top-left corner
        LOS_FOV.DrawLine(0, level.maxY - 1 - offset, 0 + offset, level.maxY - 1,
            (int x, int y, int prev_x, int prev_y) =>
            {
                tempLevel[x, y] = true;
                return true;
            });

        // draw a line in the bottom-left corner
        LOS_FOV.DrawLine(0, 0 + offset, 0 + offset, 0,
            (int x, int y, int prev_x, int prev_y) =>
            {
                tempLevel[x, y] = true;
                return true;
            });

        // draw a line in the top-right corner
        LOS_FOV.DrawLine(level.maxX - 1, level.maxY - 1 - offset, level.maxX - 1 - offset, level.maxY - 1,
            (int x, int y, int prev_x, int prev_y) =>
            {
                tempLevel[x, y] = true;
                return true;
            });

        // draw a line in the bottom-right corner
        LOS_FOV.DrawLine(level.maxX - 1, 0 + offset, level.maxX - 1 - offset, 0,
            (int x, int y, int prev_x, int prev_y) =>
            {
                tempLevel[x, y] = true;
                return true;
            });

        // find all mobs that are inside the void
        // place actual void terrain on the level
        GameObject[,] tiles = BoardManager.instance.tiles;
        GameObject[,] fog = BoardManager.instance.fog;
        List<Mob> affectedMobs = new List<Mob>();
        for (int x = 0; x < level.maxX; x++)
        {
            for (int y = 0; y < level.maxY; y++)
            {
                if (tempLevel[x, y] == true && level.mobs[x,y] != null)
                    affectedMobs.Add(level.mobs[x, y]);

                if (tempLevel[x, y] == true)
                {
                    level.terrain[x, y] = TerrainTypeEnum.terrainVoidBorder;

                    for (int i = level.items[x,y].Count - 1; i >= 0; i--)
                    {
                        BoardManager.instance.RemoveItemFromWorld(level.items[x, y][i]);
                    }

                    for (int i = level.features[x, y].Count - 1; i >= 0; i--)
                    {
                        BoardManager.instance.RemoveFeatureFromWorld(level.features[x, y][i]);
                    }

                    GameObject.Destroy(tiles[x, y]);
                    GameObject.Destroy(fog[x, y]);

                    tiles[x, y] = GameObject.Instantiate(TerrainTypes.terrainTypes[level.terrain[x, y]].prefab, new Vector3(x, y, 0f), Quaternion.identity);
                    tiles[x, y].GetComponent<SpriteRenderer>().color = TerrainTypes.terrainTypes[level.terrain[x, y]].color;
                    tiles[x, y].transform.SetParent(GameObject.Find("TilesParent").transform);

                    fog[x, y] = GameObject.Instantiate(TerrainTypes.terrainTypes[level.terrain[x, y]].prefab, new Vector3(x, y, 0), Quaternion.identity);
                    fog[x, y].GetComponent<SpriteRenderer>().color = new Color32(100, 100, 100, 255);
                    fog[x, y].GetComponent<SpriteRenderer>().sortingOrder = 10;
                    fog[x, y].transform.SetParent(GameObject.Find("FogParent").transform);
                }
            }
        }

        level.CalculateConnectivity();

        // find a new place for the affected mobs
        foreach (Mob mob in affectedMobs)
        {
            Vector2Int foundSpot;
            if (FindNearestFreeSpot(mob, out foundSpot))
            {
                mob.SetPosition(foundSpot.x, foundSpot.y);
                mob.go.transform.position = new Vector3(mob.x, mob.y, 0);
            }
            else
            {
                if (!mob.CheckDead())
                {
                    mob.curHP = 0;
                    mob.MakeDead(null, false, true, false, "Obliterated by a shrinking dimension.");
                }
            }
        }

        offset++;
    }

    public override bool CheckEvent()
    {
        return true;
    }

    public override string Description()
    {
        string str = System.String.Format("The level will completely shrink in {0} {1}", turnsLeft, (turnsLeft > 1) ? "turns" : "turn");
        return str;
    }

    public override void Initialize()
    {
        steps = (level.maxX > level.maxY) ? level.maxX : level.maxY;
        offset = 0;
    }

    public override string LineDescription()
    {
        string str = System.String.Format("Shrinking dimension");
        if ((turnsLeft - 1) % (initTurns / steps) == 0)
            str = System.String.Format("The dimension is about to shrink!");
        return str;
    }

    private bool FindNearestFreeSpot(Mob mob, out Vector2Int result)
    {
        Level level = BoardManager.instance.level;
        result = new Vector2Int(mob.x, mob.y);
        bool foundSpot = false;

        Vector2Int spot = new Vector2Int(mob.x, mob.y);
        level.CheckSurroundings(mob.x, mob.y, false,
            (int dx, int dy) =>
            {
                if (dx >= 0 && dy >= 0 & dx < level.maxX && dy < level.maxY && 
                    mob.CanMoveToPos(dx, dy).result == AttemptMoveResultEnum.moveClear)
                {
                    foundSpot = true;
                    spot = new Vector2Int(dx, dy);
                }
            });

        if (foundSpot)
        {
            result = spot;
            return true;
        }
        else
            return false;
    }
}