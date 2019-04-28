using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate bool passability_func(int dx, int dy, int prevx, int prevy);

public delegate float cost_func(int dx, int dy);

public static class Astar
{

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

    private class Location
    {
        public int x;
        public int y;
        public double f;
        public double g;
        public Location p;

        public Location(int x, int y, int ex, int ey, Location p, double g)
        {
            this.x = x;
            this.y = y;
            this.p = p;
            this.g = g;
            this.f = g + Euclidean(x, y, ex, ey);
        }

    }

    private static double Euclidean(int sx, int sy, int tx, int ty)
    {
        var x = sx - tx;
        var y = sy - ty;

        return Math.Sqrt(x * x + y * y);
    }

    private static bool LocationEqual(Location a, Location b)
    {
        if (a.x == b.x && a.y == b.y) return true;
        else return false;
    }

    private static Location FindLocationWithCoords(int x, int y, List<Location> list)
    {
        return list.Find((Location a) =>
        {
            return (a.x == x && a.y == y);
        });
    }

    public static List<Vector2Int> FindPath(int sx, int sy, int tx, int ty, passability_func passFunc, cost_func costFunc)
    {
        List<Location> openNodes = new List<Location>();
        List<Location> closedNodes = new List<Location>();
        Location startNode = new Location(sx, sy, tx, ty, null, costFunc(sx, sy));
        Location endNode = new Location(tx, ty, tx, ty, null, costFunc(tx, ty));
        List<Vector2Int> result = new List<Vector2Int>();

        openNodes.Add(startNode);
        Location curNode = null;

        do
        {
            curNode = openNodes[0];
            
            if (LocationEqual(curNode, endNode)) break;

            openNodes.RemoveAt(0);

            foreach (Vector2Int offset in offsetList)
            {
                int x = curNode.x + offset.x;
                int y = curNode.y + offset.y;

                if (!(offset.x == 0 && offset.y == 0) &&
                    (FindLocationWithCoords(x, y, closedNodes) == null) &&
                    (FindLocationWithCoords(x, y, openNodes) == null) &&
                    passFunc(x, y, curNode.x, curNode.y))
                {
                    openNodes.Add(new Location(x, y, endNode.x, endNode.y, curNode, costFunc(x, y)));
                }
            }

            closedNodes.Add(curNode);
            openNodes.Sort((Location a, Location b) =>
            {
                if (a.f < b.f) return -1;
                else if (a.f == b.f) return 0;
                else return 1;
            });
        } while (openNodes.Count != 0);

        if (openNodes.Count != 0)
        {
            curNode = openNodes[0];

            do
            {
                result.Add(new Vector2Int(curNode.x, curNode.y));
                curNode = curNode.p;
            } while (curNode != null && !LocationEqual(curNode, startNode));

            result.Reverse();

            return result;
        }
        return result;
    }

}